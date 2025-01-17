﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenAlprWebhookProcessor.Alerts;
using OpenAlprWebhookProcessor.CameraUpdateService;
using OpenAlprWebhookProcessor.Data;
using OpenAlprWebhookProcessor.Utilities;
using OpenAlprWebhookProcessor.WebhookProcessor.OpenAlprWebhook;

namespace OpenAlprWebhookProcessor.WebhookProcessor
{
    public class GroupWebhookHandler
    {
        private readonly ILogger _logger;

        private readonly IHubContext<ProcessorHub.ProcessorHub, ProcessorHub.IProcessorHub> _processorHub;

        private readonly CameraUpdateService.CameraUpdateService _cameraUpdateService;

        private readonly ProcessorContext _processorContext;

        private readonly AlertService _alertService;

        public GroupWebhookHandler(
            ILogger<GroupWebhookHandler> logger,
            CameraUpdateService.CameraUpdateService cameraUpdateService,
            ProcessorContext processorContext,
            IHubContext<ProcessorHub.ProcessorHub, ProcessorHub.IProcessorHub> processorHub,
            AlertService alertService)
        {
            _logger = logger;
            _cameraUpdateService = cameraUpdateService;
            _processorContext = processorContext;
            _processorHub = processorHub;
            _alertService = alertService;
        }

        public async Task HandleWebhookAsync(
            Webhook webhook,
            bool isBulkImport,
            CancellationToken cancellationToken)
        {
            var camera = await _processorContext.Cameras
                .Where(x => x.OpenAlprCameraId == webhook.Group.CameraId)
                .FirstOrDefaultAsync(cancellationToken);

            if (camera == null)
            {
                _logger.LogError("unknown camera: " + webhook.Group.CameraId + ", skipping.");
                return;
            }

            if (!camera.OpenAlprEnabled)
            {
                _logger.LogError("camera has OpenALPR integration disabled, skipping.");
                return;
            }

            if (webhook.Group.IsParked)
            {
                _logger.LogInformation($"parked car: {webhook.Group.BestPlateNumber}, ignoring.");
                return;
            }

            var previousPreviewGroups = await _processorContext.PlateGroups
                .Where(x => webhook.Group.Uuids.Contains(x.OpenAlprUuid))
                .ToListAsync(cancellationToken);

            PlateGroup plateGroup;
            if (previousPreviewGroups.Count > 0)
            {
                plateGroup = previousPreviewGroups[0];
                _processorContext.PlateGroups.RemoveRange(previousPreviewGroups.Skip(1));

                _logger.LogInformation("Previous preview plate exists: " + plateGroup.BestNumber + ", overwriting");
                
            }
            else
            {
                plateGroup = new PlateGroup();
            }

            plateGroup.AlertDescription = webhook.Description;
            plateGroup.PlateCoordinates = FormatLicensePlateXyCoordinates(webhook.Group.BestPlate.Coordinates);
            plateGroup.Direction = webhook.Group.TravelDirection;
            plateGroup.IsAlert = webhook.DataType == "alpr_alert";
            plateGroup.OpenAlprCameraId = webhook.Group.CameraId;
            plateGroup.OpenAlprProcessingTimeMs = Math.Round(webhook.Group.BestPlate.ProcessingTimeMs, 2);
            plateGroup.OpenAlprUuid = webhook.Group.BestUuid;
            plateGroup.BestNumber = webhook.Group.BestPlateNumber;
            plateGroup.PossibleNumbers = string.Join(",", webhook.Group.Candidates.Select(x => x.Plate).ToArray());
            plateGroup.PlatePreviewJpeg = webhook.Group.BestPlate.PlateCropJpeg;
            plateGroup.VehiclePreviewJpeg = webhook.Group.VehicleCropJpeg;
            plateGroup.Confidence = Math.Round(webhook.Group.BestPlate.Confidence, 2);
            plateGroup.ReceivedOnEpoch = webhook.Group.EpochStart;

            MapVehicle(plateGroup, webhook);

            if (previousPreviewGroups.Count == 0)
            {
                _processorContext.PlateGroups.Add(plateGroup);
            }

            await _processorContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("plate saved successfully");

            if (!isBulkImport)
            {
                if (camera.UpdateOverlayEnabled)
                {
                    var updateRequest = new CameraUpdateRequest()
                    {
                        LicensePlateImageUuid = webhook.Group.BestUuid,
                        LicensePlate = webhook.Group.BestPlateNumber,
                        LicensePlateJpeg = webhook.Group.BestPlate != null ? Convert.FromBase64String(webhook.Group.BestPlate.PlateCropJpeg) : null,
                        Id = camera.Id,
                        OpenAlprProcessingTimeMs = webhook.Group.BestPlate != null ? Math.Round(webhook.Group.BestPlate.ProcessingTimeMs, 2) : 0,
                        ProcessedPlateConfidence = webhook.Group.BestPlate != null ? Math.Round(webhook.Group.BestPlate.Confidence, 2) : 0,
                        IsAlert = webhook.DataType == "alpr_alert",
                        IsPreviewGroup = webhook.Group.IsPreview,
                        AlertDescription = webhook.Description,
                        VehicleDescription = VehicleUtilities.FormatVehicleDescription(plateGroup.VehicleYear + " " + plateGroup.VehicleMakeModel),
                    };

                    _cameraUpdateService.ScheduleOverlayRequest(updateRequest);
                }

                if (!webhook.Group.IsPreview)
                {
                    await _processorHub.Clients.All.LicensePlateRecorded(webhook.Group.BestPlateNumber);

                    var alert = await _processorContext.Alerts
                        .Where(x => x.PlateNumber.ToUpper() == webhook.Group.BestPlateNumber || plateGroup.PossibleNumbers.Contains(x.PlateNumber.ToUpper()))
                        .FirstOrDefaultAsync(cancellationToken);

                    if (alert != null)
                    {
                        var alertUpdateRequest = new AlertUpdateRequest()
                        {
                            CameraId = camera.Id,
                            Description = alert.PlateNumber + " " + alert.Description + " was seen on " + DateTimeOffset.Now.ToString("g"),
                            LicensePlateId = plateGroup.Id,
                            IsStrictMatch = alert.IsStrictMatch,
                        };

                        _alertService.AddJob(alertUpdateRequest);
                    }
                }

                var forwards = await _processorContext.WebhookForwards.ToListAsync(cancellationToken);

                foreach (var forward in forwards)
                {
                    if (forward.ForwardGroups || (forward.ForwardGroupPreviews && webhook.Group.IsPreview))
                    {
                        try
                        {
                            await WebhookForwarder.ForwardWebhookAsync(
                                webhook,
                                forward.FowardingDestination,
                                forward.IgnoreSslErrors,
                                cancellationToken);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError("failed to forward webhook to: {url}, error: {error}", forward.FowardingDestination, ex.Message);
                        }
                    }
                }
            }
        }

        private static void MapVehicle(
            PlateGroup plateGroup,
            Webhook webhook)
        {
            if (webhook.Group.Vehicle.MakeModels.First()?.Confidence > 30)
            {
                plateGroup.VehicleMakeModel = webhook.Group.Vehicle.MakeModels.First()?.Name;

                if (webhook.Group.Vehicle.Colors.First()?.Confidence > 30)
                {
                    plateGroup.VehicleColor = webhook.Group.Vehicle.Colors.First()?.Name;
                }

                if (webhook.Group.Vehicle.Makes.First()?.Confidence > 30)
                {
                    plateGroup.VehicleMake = webhook.Group.Vehicle.Makes.First()?.Name;
                }

                if (webhook.Group.Vehicle.BodyTypes.First()?.Confidence > 30)
                {
                    plateGroup.VehicleType = webhook.Group.Vehicle.BodyTypes.First()?.Name;
                }

                if (webhook.Group.Vehicle.Years.First()?.Confidence > 30)
                {
                    plateGroup.VehicleYear = webhook.Group.Vehicle.Years.First()?.Name;
                }

                if (webhook.Group.BestPlate.RegionConfidence > 30)
                {
                    plateGroup.VehicleRegion = webhook.Group.BestPlate.Region;
                }
            }
        }

        private static string FormatLicensePlateXyCoordinates(List<Coordinate> coordinates)
        {
            return VehicleUtilities.FormatLicensePlateImageCoordinates(
                new List<int>()
                {
                    coordinates[0].X,
                    coordinates[1].X,
                    coordinates[2].X,
                    coordinates[3].X,
                },
                new List<int>()
                {
                    coordinates[0].Y,
                    coordinates[1].Y,
                    coordinates[2].Y,
                    coordinates[3].Y,
                });
        }
    }
}
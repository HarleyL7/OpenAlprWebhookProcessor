﻿using OpenAlprWebhookProcessor.Data;
using System;
using System.Collections.Generic;

namespace OpenAlprWebhookProcessor.LicensePlates
{
    public static class PlateMapper
    {
        public static LicensePlate MapPlate(
            PlateGroup plate,
            List<string> platesToIgnore,
            List<string> platesToAlert)
        {
            return new LicensePlate()
            {
                AlertDescription = plate.AlertDescription,
                Direction = plate.Direction,
                ImageUrl = new Uri($"/images/{plate.OpenAlprUuid}.jpg", UriKind.Relative),
                CropImageUrl = new Uri($"/images/crop/{plate.OpenAlprUuid}?{plate.PlateCoordinates}", UriKind.Relative),
                IsAlert = platesToAlert.Contains(plate.Number),
                IsIgnore = platesToIgnore.Contains(plate.Number),
                LicensePlateJpegBase64 = plate.Jpeg,
                OpenAlprCameraId = plate.OpenAlprCameraId,
                OpenAlprProcessingTimeMs = plate.OpenAlprProcessingTimeMs,
                PlateNumber = plate.Number,
                ProcessedPlateConfidence = plate.Confidence,
                ReceivedOn = DateTimeOffset.FromUnixTimeMilliseconds(plate.ReceivedOnEpoch),
                VehicleDescription = plate.VehicleDescription,
            };
        }
    }
}
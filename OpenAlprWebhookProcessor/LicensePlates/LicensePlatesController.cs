﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenAlprWebhookProcessor.LicensePlates.DeletePlate;
using OpenAlprWebhookProcessor.LicensePlates.GetLicensePlateBreakdown;
using OpenAlprWebhookProcessor.LicensePlates.GetLicensePlateCounts;
using OpenAlprWebhookProcessor.LicensePlates.SearchLicensePlates;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAlprWebhookProcessor.LicensePlates
{
    [Authorize]
    [ApiController]
    [Route("licensePlates")]
    public class LicensePlatesController : ControllerBase
    {
        private readonly SearchLicensePlateHandler _searchLicensePlateHandler;

        private readonly GetLicensePlateCountsHandler _getLicensePlateCountsHandler;

        private readonly DeleteLicensePlateGroupRequestHandler _deleteLicensePlateGroupHandler;

        private readonly GetLicensePlateBreakdownHandler _getLicensePlateBreakdownHandler;

        public LicensePlatesController(
            SearchLicensePlateHandler searchLicensePlateHandler,
            GetLicensePlateCountsHandler getLicensePlateCountsHandler,
            DeleteLicensePlateGroupRequestHandler deleteLicensePlateGroupHandler,
            GetLicensePlateBreakdownHandler getLicensePlateBreakdownHandler)
        {
            _searchLicensePlateHandler = searchLicensePlateHandler;
            _getLicensePlateCountsHandler = getLicensePlateCountsHandler;
            _deleteLicensePlateGroupHandler = deleteLicensePlateGroupHandler;
            _getLicensePlateBreakdownHandler = getLicensePlateBreakdownHandler;
        }

        [HttpPost("search")]
        public async Task<SearchLicensePlateResponse> SearchPlates(
            [FromBody] SearchLicensePlateRequest request,
            CancellationToken cancellationToken)
        {
            return await _searchLicensePlateHandler.HandleAsync(
                request,
                cancellationToken);
        }

        [HttpDelete("{plateId}")]
        public async Task DeletePlate(
            Guid plateId,
            CancellationToken cancellationToken)
        {
            await _deleteLicensePlateGroupHandler.HandleAsync(
                plateId,
                cancellationToken);
        }
        
        [HttpGet("counts/{numberOfDays}")]
        public async Task<GetLicensePlateBreakdownResponse> GetLicensePlateCounts(
            int numberOfDays,
            CancellationToken cancellationToken)
        {
            return await _getLicensePlateBreakdownHandler.HandleAsync(
                new GetLicensePlateBreakdownRequest(),
                cancellationToken);
        }
    }
}

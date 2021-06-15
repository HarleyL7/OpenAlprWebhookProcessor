using Microsoft.EntityFrameworkCore;
using OpenAlprWebhookProcessor.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAlprWebhookProcessor.LicensePlates.GetLicensePlateCounts
{
    public class GetLicensePlateCountsHandler
    {
        private readonly ProcessorContext _processerContext;

        public GetLicensePlateCountsHandler(ProcessorContext processorContext)
        {
            _processerContext = processorContext;
        }

        public async Task<GetLicensePlateCountsResponse> HandleAsync(
            GetLicensePlateCountsRequest request,
            CancellationToken cancellationToken)
        {
            var requestRange = DateTimeOffset.UtcNow.AddDays(request.NumberOfDays * -1).ToUnixTimeMilliseconds();

            var results = await _processerContext.PlateGroups
                .Where(x => x.ReceivedOnEpoch > requestRange)
                .Select(y => y.ReceivedOnEpoch)
                .ToListAsync(cancellationToken);

            var groupedResults = results.GroupBy(x => DateTimeOffset.FromUnixTimeMilliseconds(x).Date);
            var parsedResults = new List<DayCount>();

            foreach (var date in groupedResults)
            {
                parsedResults.Add(new DayCount()
                {
                    Value = date.Count(),
                    Date = date.Key,
                });
            }

            return new GetLicensePlateCountsResponse()
            {
                Counts = parsedResults,
            };
        }
    }
}

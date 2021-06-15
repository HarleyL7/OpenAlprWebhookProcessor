using Microsoft.EntityFrameworkCore;
using OpenAlprWebhookProcessor.Data;
using OpenAlprWebhookProcessor.LicensePlates.GetLicensePlateCounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAlprWebhookProcessor.LicensePlates.GetLicensePlateBreakdown
{
    public class GetLicensePlateBreakdownHandler
    {
        private readonly ProcessorContext _processerContext;

        public GetLicensePlateBreakdownHandler(ProcessorContext processorContext)
        {
            _processerContext = processorContext;
        }

        public async Task<GetLicensePlateBreakdownResponse> HandleAsync(
            GetLicensePlateBreakdownRequest request,
            CancellationToken cancellationToken)
        {
            var results = await _processerContext.PlateGroups
                .Select(y => y.ReceivedOnEpoch)
                .ToListAsync(cancellationToken);

            var groupedResults = results.GroupBy(x => DateTimeOffset.FromUnixTimeMilliseconds(x).Date).OrderBy(x => x.Key).ToList();
            var parsedResults = new List<Week>();

            for (var i = DateTimeOffset.Now; i > groupedResults.First().Key; i = i.AddDays(-1))
            {
                if (!groupedResults.Any(x => x.Key.Date == i.Date))
                {
                    var emptyGroup = new List<long>()
                    {
                        i.ToUnixTimeMilliseconds()
                    };

                    var grouped = emptyGroup.GroupBy(x => DateTimeOffset.FromUnixTimeMilliseconds(x).Date).ToList();

                    groupedResults.Add(grouped.First());
                }
            }

            for (int i = 0; i < 52; i++)
            {
                var week = new Week();
                week.Series = new List<DayCount>();
                var currentWeek = groupedResults.Skip(i * 7).Take(7);

                if (currentWeek.Any())
                {
                    week.Name = currentWeek.First().Key.ToString();
                    foreach (var day in currentWeek)
                    {
                        week.Series.Add(new DayCount()
                        {
                            Value = day.Count(),
                            Name = day.Key.DayOfWeek.ToString(),
                            Date = day.Key,
                        });
                    }

                    parsedResults.Add(week);
                }
            }

            return new GetLicensePlateBreakdownResponse()
            {
                Weeks = parsedResults,
            };
        }
    }

    public class Week
    {
        public string Name { get; set; }

        public List<DayCount> Series { get; set; }
    }
}

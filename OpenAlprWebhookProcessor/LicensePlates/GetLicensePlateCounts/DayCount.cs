using System;

namespace OpenAlprWebhookProcessor.LicensePlates.GetLicensePlateCounts
{
    public class DayCount
    {
        public DateTimeOffset Date { get; set; }

        public string Name { get; set; }

        public int Value { get; set; }
    }
}

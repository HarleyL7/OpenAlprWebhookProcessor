using OpenAlprWebhookProcessor.LicensePlates.GetLicensePlateCounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenAlprWebhookProcessor.LicensePlates.GetLicensePlateBreakdown
{
    public class GetLicensePlateBreakdownResponse
    {
        public List<Week> Weeks { get; set; }
    }
}

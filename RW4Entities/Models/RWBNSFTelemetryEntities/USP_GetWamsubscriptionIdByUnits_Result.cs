using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RW4Entities.Models.RWBNSFTelemetryEntities
{
    public partial class USP_GetWamsubscriptionIdByUnits_Result
    {
        public Nullable<long> WamSubscriptionId { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public long UnitMasterId { get; set; }
        public long PrenoteId { get; set; }
        public Nullable<System.Guid> WamSubscriptionGuid { get; set; }
        public string APIStartDttm { get; set; }
    }
}

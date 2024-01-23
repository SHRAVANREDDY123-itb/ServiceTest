using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RW4Entities.Models.RWOBDistributorsEntities
{
    public partial class USP_MirgrateExstingWamSubscriptionToNewSubscriptionUrl_Result
    {
        public DateTime ETSDt { get; set; }
        public long PrenoteId { get; set; }
        public long UnitMasterId { get; set; }
        public string UnitNumber { get; set; }
    }
}

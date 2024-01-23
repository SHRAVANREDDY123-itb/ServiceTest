using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RW4Entities.Models.RWOBDistributorsEntities
{
    public partial class USP_OBDist_GetInsDetailsByEventID_Result
    {
        public long SOId { get; set; }
        public long? PreNoteId { get; set; }
        public string? PreNoteNbr { get; set; }
        public string? UnitNumber { get; set; }
        public string? GensetNumber { get; set; }
        public DateTime? SOSvcDtTm { get; set; }
    }
}

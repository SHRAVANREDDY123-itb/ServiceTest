using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RW4Entities.Models.RWOBDistributorsEntities
{
    public partial class USP_OBDist_GetInsTripDetails_Result
    {
        public long FacilityId { get; set; }
        public string? FacilityTypeCd { get; set; }
        public string? FacilityNm { get; set; }
    }
}

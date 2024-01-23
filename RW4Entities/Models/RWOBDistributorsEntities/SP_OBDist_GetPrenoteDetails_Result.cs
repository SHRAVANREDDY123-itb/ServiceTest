using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RW4Entities.Models.RWOBDistributorsEntities
{
    public partial class USP_OBDist_GetPrenoteDetails_Result
    {
        public long PreNoteId { get; set; }
        public DateTime ETSDt { get; set; }
        public long UnitMasterId { get; set; }
    }
}

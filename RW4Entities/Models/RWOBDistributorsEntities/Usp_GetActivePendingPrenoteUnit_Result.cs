using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RW4Entities.Models.RWOBDistributorsEntities
{
    public partial class Usp_GetActivePendingPrenoteUnit_Result
    {
        public long PreNoteId { get; set; }
        public long PreNoteUnitId { get; set; }
        public long UnitMasterId { get; set; }
    }
}

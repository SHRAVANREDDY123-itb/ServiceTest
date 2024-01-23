using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RW4Entities.Models.RWOBDistributorsEntities
{
    public partial class USP_GetPreNoteUnitStatusByEventId_Result
    {
        public long PreNoteId { get; set; }
        public string PreNoteStatusCd { get; set; }
        public long PreNoteUnitId { get; set; }
        public string PreNoteUnitStatusCd { get; set; }
    }
}

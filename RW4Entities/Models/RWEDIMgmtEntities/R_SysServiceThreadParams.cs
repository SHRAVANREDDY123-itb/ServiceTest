using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RW4Entities.Models.RWEDIMgmtEntities
{
    public partial class R_SysServiceThreadParams
    {
        [Required]
        [StringLength(15)]
        [Unicode(false)]
        public string SysParamCd { get; set; }

        [StringLength(100)]
        [Unicode(false)]
        public string SysParamVal { get; set; }

        public long SysServiceThreadId { get; set; }

        [Key]
        public int SysServiceThreadParamID { get; set; }
    }
}

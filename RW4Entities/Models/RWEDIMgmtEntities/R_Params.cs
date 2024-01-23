using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RW4Entities.Models.RWEDIMgmtEntities
{
    public partial class R_Params
    {
        [Key]
        public int ParamID { get; set; }

        [Unicode(false)]
        public string? ParamCd { get; set; }

        [Required]
        [StringLength(100)]
        [Unicode(false)]
        public string? Description { get; set; }
    }
}

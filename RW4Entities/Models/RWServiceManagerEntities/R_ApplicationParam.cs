using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RW4Entities.Models.RWServiceManagerEntities
{
    public partial class R_ApplicationParam
    {
        [Key]
        public int ParamID { get; set; }

        public string? Value { get; set; }
    }
}

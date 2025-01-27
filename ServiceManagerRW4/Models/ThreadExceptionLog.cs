using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceManagerRW4.Models
{
    public class ThreadExceptionLog
    {
        
            
        public long SysServiceThreadId { get; set; }

        public string? ThreadException { get; set; }

        
        public DateTime? CreateDtTm { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceManagerRW4.Models
{
    public class SyServiceThreadConfiguration
    {
        public long threadID { get; set; }
        public string? AssemblyFullName { get; set; }
        public int ThreadSleepTm { get; set; } = 1;
    }
}

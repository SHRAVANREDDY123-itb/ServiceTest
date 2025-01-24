using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceManagerRW4.Models
{
    public class SysServiceThread
    {
        public long SysServiceThreadId { get; set; }
        public long ServiceId { get; set; }
        public string CurrentStatusCode { get; set; }
        public string RequestedStatusCode { get; set; }
        public DateTime? LastStarted { get; set; }
        public DateTime? LastStopped { get; set; }
        public DateTime? CurrentProcessingStart { get; set; }
        public int? SleepTime { get; set; }
        public bool ReloadFlag { get; set; }
        public string MethodName { get; set; }
        public string IsActive { get; set; }
        public int? RetryCount { get; set; }
        public string ThreadType { get; set; }
        public string TaskTime { get; set; }
        public string IsSuccessful { get; set; }
    }

}

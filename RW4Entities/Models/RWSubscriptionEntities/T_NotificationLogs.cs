using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RW4Entities.Models.RWSubscriptionEntities
{
    public partial class T_NotificationLogs
    {
        public long NotificationLogId { get; set; }

        public long? SubscriptionId { get; set; }

        public string SubscriptionNm { get; set; }

        public long? EventID { get; set; }

        public string MessageBody { get; set; }

        public string Subject { get; set; }

        public string Recipient { get; set; }

        public bool? RetransmitCount { get; set; }

        public bool IsSuccess { get; set; }

        public string ErrorText { get; set; }

        public long SysServiceThreadId { get; set; }

        public DateTime CreatedDttm { get; set; }

        public DateTime UpdateDttm { get; set; }

        public long? EDIEventID { get; set; }
    }
}

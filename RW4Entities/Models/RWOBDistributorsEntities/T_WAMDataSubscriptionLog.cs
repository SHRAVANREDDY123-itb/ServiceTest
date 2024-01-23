using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RW4Entities.Models.RWOBDistributorsEntities
{
    public partial class T_WAMDataSubscriptionLog
    {
        [Key]
        public long WAMDataSubscriptionLogId { get; set; }

        public long? WamSubscriptionId { get; set; }

        public long? WamQueueId { get; set; }

        [Unicode(false)]
        public string WamData { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CreateDt { get; set; }
    }
}

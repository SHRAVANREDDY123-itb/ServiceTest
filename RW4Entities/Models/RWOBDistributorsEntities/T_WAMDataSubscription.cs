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
    public partial class T_WAMDataSubscription
    {
        [Key]
        public long WAMDataSubscriptionId { get; set; }

        public long PrenoteId { get; set; }

        public long UnitMasterId { get; set; }

        public long? WamSubscriptionId { get; set; }

        [StringLength(1)]
        [Unicode(false)]
        public string StatusCd { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? StartDate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? EndDate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CreateDt { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? ModifyDt { get; set; }

        public Guid? WamSubscriptionGuid { get; set; }

        [StringLength(100)]
        [Unicode(false)]
        public string? APIStartDttm { get; set; }
    }
}

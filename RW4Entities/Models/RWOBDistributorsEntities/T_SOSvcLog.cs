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
    public partial class T_SOSvcLog
    {
        [Key]
        public long SOSvcLogID { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime SOSvcDtTm { get; set; }

        [Required]
        [StringLength(1)]
        [Unicode(false)]
        public string SOSvcLogStatusCd { get; set; }

        [StringLength(2500)]
        [Unicode(false)]
        public string Comments { get; set; }

        public long CreatedBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime CreatedDtTm { get; set; }

        public long ModifiedBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime ModifiedDtTm { get; set; }

        [Required]
        public byte[] VerNbr { get; set; }

        public long SOid { get; set; }

        [Required]
        [StringLength(3)]
        [Unicode(false)]
        public string InspectionTypeCd { get; set; }

        [StringLength(10)]
        [Unicode(false)]
        public string FuelLevel { get; set; }

        [StringLength(500)]
        [Unicode(false)]
        public string SVCImageAttach { get; set; }

        [Column(TypeName = "decimal(18, 8)")]
        public decimal? Longitude { get; set; }

        [Column(TypeName = "decimal(18, 8)")]
        public decimal? Latitude { get; set; }

        [StringLength(5)]
        [Unicode(false)]
        public string SVCSourceCd { get; set; }

        public Guid SOSvcLogGUID { get; set; }

        [StringLength(1)]
        [Unicode(false)]
        public string ServiceRateTypeCD { get; set; }

        public long? PreNoteUnitHistoryId { get; set; }
    }
}

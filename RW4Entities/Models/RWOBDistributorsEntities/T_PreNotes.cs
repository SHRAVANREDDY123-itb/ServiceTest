﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RW4Entities.Models.RWOBDistributorsEntities;

public partial class T_PreNotes
{
    [Key]
    public long PreNoteId { get; set; }

    public long PSPFOContractId { get; set; }

    [Required]
    [StringLength(20)]
    [Unicode(false)]
    public string PreNoteNbr { get; set; }

    [Required]
    [StringLength(1)]
    [Unicode(false)]
    public string PreNoteTypeCd { get; set; }

    public long FOId { get; set; }

    public long PSPId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime IssueDt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime ETSDt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ETCDt { get; set; }

    [Required]
    [StringLength(3)]
    [Unicode(false)]
    public string PreNoteStatusCd { get; set; }

    [StringLength(250)]
    [Unicode(false)]
    public string FORefNbr { get; set; }

    public long? RouteId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedDtTm { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime ModifiedDtTm { get; set; }

    public long CreatedBy { get; set; }

    public long ModifiedBy { get; set; }

    [StringLength(25)]
    [Unicode(false)]
    public string BOL { get; set; }

    [Required]
    public byte[] VerNbr { get; set; }

    public bool IsError { get; set; }

    public bool? IsRouteChanged { get; set; }

    [Required]
    [StringLength(1)]
    [Unicode(false)]
    public string Source { get; set; }

    [InverseProperty("PreNote")]
    public virtual ICollection<T_PreNoteEvents> T_PreNoteEvents { get; set; } = new List<T_PreNoteEvents>();

    [InverseProperty("PreNote")]
    public virtual ICollection<T_PreNoteUnits> T_PreNoteUnits { get; set; } = new List<T_PreNoteUnits>();

    [InverseProperty("PreNote")]
    public virtual ICollection<T_RailincCERLog> T_RailincCERLog { get; set; } = new List<T_RailincCERLog>();

    [InverseProperty("PreNote")]
    public virtual ICollection<T_UnitCurrentCondition> T_UnitCurrentCondition { get; set; } = new List<T_UnitCurrentCondition>();
}
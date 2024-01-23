﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RW4Entities.Models.RWOBDistributorsEntities;

public partial class R_UnitMaster
{
    [Key]
    public long UnitMasterId { get; set; }

    [StringLength(3)]
    [Unicode(false)]
    public string UnitTypeCd { get; set; }

    [Required]
    [StringLength(20)]
    [Unicode(false)]
    public string UnitNumber { get; set; }

    public int? UnitTankSize { get; set; }

    public int? UnitSize { get; set; }

    public int? UnitHeight { get; set; }

    public long? FOId { get; set; }

    public bool IsActive { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedDtTm { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime ModifiedDtTm { get; set; }

    public long CreatedBy { get; set; }

    public long ModifiedBy { get; set; }

    [StringLength(1)]
    [Unicode(false)]
    public string SizeUOM { get; set; }

    [InverseProperty("UnitMaster")]
    public virtual ICollection<T_PreNoteUnits> T_PreNoteUnits { get; set; } = new List<T_PreNoteUnits>();

    [InverseProperty("UnitMaster")]
    public virtual T_UnitCurrentCondition T_UnitCurrentCondition { get; set; }

    [InverseProperty("UnitMaster")]
    public virtual ICollection<T_UnitEvents> T_UnitEvents { get; set; } = new List<T_UnitEvents>();
}
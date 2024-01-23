﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RW4Entities.Models.RWOBDistributorsEntities;

public partial class R_EventMaster
{
    [Key]
    public long EventMasterId { get; set; }

    [Required]
    [StringLength(2)]
    [Unicode(false)]
    public string EventCategoryCd { get; set; }

    [Required]
    [StringLength(10)]
    [Unicode(false)]
    public string EventCd { get; set; }

    [StringLength(2000)]
    [Unicode(false)]
    public string EventDsc { get; set; }

    public bool IsActive { get; set; }

    public bool IsVisibleOnUI { get; set; }

    [InverseProperty("EventMaster")]
    public virtual ICollection<T_Events> T_Events { get; set; } = new List<T_Events>();

    [InverseProperty("C_EventMaster")]
    public virtual ICollection<T_UnitCurrentCondition> T_UnitCurrentCondition { get; set; } = new List<T_UnitCurrentCondition>();
}
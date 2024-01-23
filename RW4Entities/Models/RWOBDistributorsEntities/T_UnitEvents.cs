﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RW4Entities.Models.RWOBDistributorsEntities;

public partial class T_UnitEvents
{
    [Key]
    public long UnitEventId { get; set; }

    public long EventId { get; set; }

    public long UnitMasterId { get; set; }

    [ForeignKey("EventId")]
    [InverseProperty("T_UnitEvents")]
    public virtual T_Events Event { get; set; }

    [InverseProperty("UnitEvent")]
    public virtual ICollection<T_UnitCurrentCondition> T_UnitCurrentCondition { get; set; } = new List<T_UnitCurrentCondition>();

    [ForeignKey("UnitMasterId")]
    [InverseProperty("T_UnitEvents")]
    public virtual R_UnitMaster UnitMaster { get; set; }
}
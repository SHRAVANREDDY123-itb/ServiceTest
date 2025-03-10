﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RW4Entities.Models.RWOBDistributorsEntities;

public partial class T_PreNoteEvents
{
    [Key]
    public long PreNoteEventId { get; set; }

    public long PreNoteId { get; set; }

    public long EventId { get; set; }

    [ForeignKey("EventId")]
    [InverseProperty("T_PreNoteEvents")]
    public virtual T_Events Event { get; set; }

    [ForeignKey("PreNoteId")]
    [InverseProperty("T_PreNoteEvents")]
    public virtual T_PreNotes PreNote { get; set; }
}
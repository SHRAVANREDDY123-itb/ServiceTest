﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RW4Entities.Models;

public partial class T_ThreadExceptionLog
{
    /// <summary>
    /// Thread Log Primary Key
    /// </summary>
    [Key]
    public long SysServiceThreadLogId { get; set; }

    /// <summary>
    /// Parent Thread Unique Key
    /// </summary>
    public long SysServiceThreadId { get; set; }

    /// <summary>
    /// Thread Exception Text
    /// </summary>
    [Column(TypeName = "text")]
    public string ThreadException { get; set; }

    /// <summary>
    /// Date &amp; Time Exception ws recorded
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime? CreateDtTm { get; set; }
}
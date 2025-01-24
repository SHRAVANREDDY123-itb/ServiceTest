﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RW4Entities.Models;

public partial class R_SysServiceThreads
{
    /// <summary>
    /// Service Thread Unique Identifier
    /// </summary>
    [Key]
    public long SysServiceThreadId { get; set; }

    /// <summary>
    /// Thread Parent Service Unique Identifier
    /// </summary>
    public long SysServiceId { get; set; }

    /// <summary>
    /// Thread Current Running Status
    /// </summary>
    [StringLength(1)]
    [Unicode(false)]
    public string CurrentStatusCd { get; set; }

    /// <summary>
    /// Thread Requested Running Status
    /// </summary>
    [StringLength(1)]
    [Unicode(false)]
    public string RequestedStatusCd { get; set; }

    /// <summary>
    /// When did Service started last time
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime? LastStartedDtTm { get; set; }

    /// <summary>
    /// When did Service stopped last time
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime? LastStoppedDtTm { get; set; }

    /// <summary>
    /// When did Thread processing started last time
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime? CurrentProcessingStartDtTm { get; set; }

    /// <summary>
    /// Sleep time between consicutive processings
    /// </summary>
    public int ThreadSleepTm { get; set; }

    /// <summary>
    /// Y = Load the Service configuration again.
    /// </summary>
    [Required]
    [StringLength(1)]
    [Unicode(false)]
    public string ReLoadFlg { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string MethodNm { get; set; }

    [StringLength(1)]
    [Unicode(false)]
    public string IsActive { get; set; }

    public int? Retries { get; set; }

    [StringLength(1)]
    [Unicode(false)]
    public string ThreadType { get; set; }

    [StringLength(5)]
    [Unicode(false)]
    public string TaskTm { get; set; }

    [StringLength(1)]
    [Unicode(false)]
    public string IsSuccesful { get; set; }
}
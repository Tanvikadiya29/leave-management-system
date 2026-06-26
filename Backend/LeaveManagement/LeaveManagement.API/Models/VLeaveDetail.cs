using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.API.Models;

[Keyless]
public partial class VLeaveDetail
{
    [Column("id")]
    public int? Id { get; set; }

    [Column("employee_name")]
    public string? EmployeeName { get; set; }

    [Column("email")]
    [StringLength(200)]
    public string? Email { get; set; }

    [Column("from_date")]
    public DateOnly? FromDate { get; set; }

    [Column("to_date")]
    public DateOnly? ToDate { get; set; }

    [Column("total_days")]
    public int? TotalDays { get; set; }

    [Column("reason")]
    public string? Reason { get; set; }

    [Column("status")]
    [StringLength(20)]
    public string? Status { get; set; }

    [Column("remarks")]
    public string? Remarks { get; set; }

    [Column("created_at", TypeName = "timestamp without time zone")]
    public DateTime? CreatedAt { get; set; }
}

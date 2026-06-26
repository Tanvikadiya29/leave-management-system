using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.API.Models;

[Table("leave_requests")]
[Index("EmployeeId", Name = "idx_leave_requests_employee_id")]
public partial class LeaveRequest
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("employee_id")]
    public int EmployeeId { get; set; }

    [Column("from_date")]
    public DateOnly FromDate { get; set; }

    [Column("to_date")]
    public DateOnly ToDate { get; set; }

    [Column("reason")]
    public string Reason { get; set; } = null!;

    [Column("status")]
    [StringLength(20)]
    public string Status { get; set; } = null!;

    [Column("reviewed_by")]
    public int? ReviewedBy { get; set; }

    [Column("reviewed_at", TypeName = "timestamp without time zone")]
    public DateTime? ReviewedAt { get; set; }

    [Column("remarks")]
    public string? Remarks { get; set; }

    [Column("created_at", TypeName = "timestamp without time zone")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp without time zone")]
    public DateTime UpdatedAt { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("LeaveRequestEmployees")]
    public virtual User Employee { get; set; } = null!;

    [ForeignKey("ReviewedBy")]
    [InverseProperty("LeaveRequestReviewedByNavigations")]
    public virtual User? ReviewedByNavigation { get; set; }
}

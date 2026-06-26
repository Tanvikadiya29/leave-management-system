using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.API.Models;

[Table("users")]
[Index("Email", Name = "idx_users_email")]
[Index("Email", Name = "users_email_key", IsUnique = true)]
public partial class User
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("first_name")]
    [StringLength(100)]
    public string FirstName { get; set; } = null!;

    [Column("last_name")]
    [StringLength(100)]
    public string LastName { get; set; } = null!;

    [Column("email")]
    [StringLength(200)]
    public string Email { get; set; } = null!;

    [Column("password_hash")]
    public string PasswordHash { get; set; } = null!;

    [Column("role_id")]
    public int RoleId { get; set; }

    [Column("department")]
    [StringLength(100)]
    public string? Department { get; set; }

    [Column("designation")]
    [StringLength(100)]
    public string? Designation { get; set; }

    [Column("date_of_joining")]
    public DateOnly? DateOfJoining { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }

    [Column("created_at", TypeName = "timestamp without time zone")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp without time zone")]
    public DateTime UpdatedAt { get; set; }

    [InverseProperty("Employee")]
    public virtual ICollection<LeaveRequest> LeaveRequestEmployees { get; set; } = new List<LeaveRequest>();

    [InverseProperty("ReviewedByNavigation")]
    public virtual ICollection<LeaveRequest> LeaveRequestReviewedByNavigations { get; set; } = new List<LeaveRequest>();

    [ForeignKey("RoleId")]
    [InverseProperty("Users")]
    public virtual Role Role { get; set; } = null!;
}

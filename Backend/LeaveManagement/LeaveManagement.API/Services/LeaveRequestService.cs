using LeaveManagement.API.Data;
using LeaveManagement.API.DTOs;
using LeaveManagement.API.Models;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.API.Services;

public class LeaveRequestService(LeaveManagementDbContext db)
{
    private readonly LeaveManagementDbContext _db = db;

    private LeaveRequestDto ToDto(LeaveRequest l)
    {
        return new LeaveRequestDto
        {
            Id = l.Id,
            EmployeeId = l.EmployeeId,
            EmployeeName = l.Employee.FirstName + " " + l.Employee.LastName,
            FromDate = l.FromDate.ToString("yyyy-MM-dd"),
            ToDate = l.ToDate.ToString("yyyy-MM-dd"),
            Reason = l.Reason,
            Status = l.Status,
            Remarks = l.Remarks,
            CreatedAt = l.CreatedAt.ToString("yyyy-MM-dd")
        };
    }

    public async Task<List<LeaveRequestDto>> GetAllAsync()
    {
        var leaves = await _db.LeaveRequests
            .Include(l => l.Employee)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();

        return leaves.Select(l => ToDto(l)).ToList();
    }

    public async Task<List<LeaveRequestDto>> GetByEmployeeAsync(int employeeId)
    {
        var leaves = await _db.LeaveRequests
            .Include(l => l.Employee)
            .Where(l => l.EmployeeId == employeeId)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();

        return leaves.Select(l => ToDto(l)).ToList();
    }

    public async Task<(bool Success, string Message)> CreateAsync(int employeeId, CreateLeaveRequestDto dto)
    {
        var employee = await _db.Users.FindAsync(employeeId);
        if (employee == null || employee.IsActive == false)
            return (false, "Employee not found.");

        if (!DateOnly.TryParse(dto.FromDate, out var fromDate) ||
            !DateOnly.TryParse(dto.ToDate, out var toDate))
            return (false, "Invalid date format. Use YYYY-MM-DD.");

        if (toDate < fromDate)
            return (false, "To Date must be on or after From Date.");

        if (string.IsNullOrWhiteSpace(dto.Reason))
            return (false, "Reason is required.");

        // Overlapping leave (Pending or Approved)
        bool hasOverlap = await _db.LeaveRequests.AnyAsync(l =>
            l.EmployeeId == employeeId &&
            (l.Status == "Pending" || l.Status == "Approved") &&
            l.FromDate <= toDate &&
            l.ToDate >= fromDate);

        if (hasOverlap)
            return (false, "You already have a leave request for these dates.");

        var leave = new LeaveRequest
        {
            EmployeeId = employeeId,
            FromDate = fromDate,
            ToDate = toDate,
            Reason = dto.Reason,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.LeaveRequests.Add(leave);
        await _db.SaveChangesAsync();

        return (true, "Leave request submitted.");
    }

    public async Task<(bool Success, string Message)> ReviewAsync(int leaveId, int adminId, ReviewLeaveDto dto)
    {
        var leave = await _db.LeaveRequests.FindAsync(leaveId);
        if (leave == null)
            return (false, "Leave request not found.");

        if (leave.Status != "Pending")
            return (false, "Only pending requests can be reviewed.");

        if (dto.Status != "Approved" && dto.Status != "Rejected")
            return (false, "Status must be Approved or Rejected.");

        leave.Status = dto.Status;
        leave.ReviewedBy = adminId;
        leave.ReviewedAt = DateTime.UtcNow;
        leave.Remarks = dto.Remarks;
        leave.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return (true, $"Leave {dto.Status.ToLower()}.");
    }
}
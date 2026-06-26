using LeaveManagement.API.Data;
using LeaveManagement.API.DTOs;
using LeaveManagement.API.Models;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.API.Services;

public class EmployeeService(LeaveManagementDbContext db)
{
    private readonly LeaveManagementDbContext _db = db;

    public async Task<List<EmployeeDto>> GetAllAsync()
    {
        var users = await _db.Users
            .Where(u => u.RoleId == 2 && u.IsActive == true)
            .ToListAsync();

        var result = new List<EmployeeDto>();

        foreach (var u in users)
        {
            result.Add(new EmployeeDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Department = u.Department,
                Designation = u.Designation
            });
        }

        return result;
    }

    public async Task<EmployeeDto?> GetByIdAsync(int id)
    {
        var u = await _db.Users.FindAsync(id);

        if (u == null || u.IsActive == false)
            return null;

        return new EmployeeDto
        {
            Id = u.Id,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Email = u.Email,
            Department = u.Department,
            Designation = u.Designation
        };
    }

    public async Task<(bool Success, string Message)> CreateAsync(RegisterDto dto)
    {
        bool emailTaken = await _db.Users.AnyAsync(u => u.Email == dto.Email);
        if (emailTaken)
            return (false, "This email is already registered.");

        var newEmployee = new User
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            RoleId = 2,
            Department = dto.Department,
            Designation = dto.Designation,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        if (!string.IsNullOrEmpty(dto.DateOfJoining))
            newEmployee.DateOfJoining = DateOnly.Parse(dto.DateOfJoining);

        _db.Users.Add(newEmployee);
        await _db.SaveChangesAsync();

        return (true, "Employee added successfully.");
    }

    public async Task<(bool Success, string Message)> DeleteAsync(int id)
    {
        var user = await _db.Users.FindAsync(id);

        if (user == null)
            return (false, "Employee not found.");

        user.IsActive = false;
        user.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return (true, "Employee removed.");
    }
}
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
            .OrderByDescending(u => u.CreatedAt)
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

        if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 6)
            return (false, "Password must be at least 6 characters.");

        bool hasUpper = dto.Password.Any(char.IsUpper);
        bool hasDigit = dto.Password.Any(char.IsDigit);

        if (!hasUpper || !hasDigit)
            return (false, "Password must have at least one uppercase letter and one number.");

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
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
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
        user.UpdatedAt = DateTime.Now;

        await _db.SaveChangesAsync();

        return (true, "Employee removed.");
    }

    public async Task<(bool Success, string Message)> UpdateAsync(int id, UpdateEmployeeDto dto)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null || user.IsActive == false)
            return (false, "Employee not found.");

        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.Department = dto.Department;
        user.Designation = dto.Designation;
        user.UpdatedAt = DateTime.Now;

        await _db.SaveChangesAsync();
        return (true, "Employee updated.");
    }
}
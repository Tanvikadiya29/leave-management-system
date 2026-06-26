using LeaveManagement.API.Data;
using LeaveManagement.API.DTOs;
using LeaveManagement.API.Models;
using LeaveManagement.API.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LeaveManagement.Tests;

public class LeaveRequestServiceTests
{
    private LeaveManagementDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<LeaveManagementDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var db = new LeaveManagementDbContext(options);

        db.Roles.Add(new Role { Id = 2, RoleName = "Employee" });

        db.Users.Add(new User
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@test.com",
            PasswordHash = "hash",
            RoleId = 2,
            IsActive = true,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        });

        db.SaveChanges();

        return db;
    }

    [Fact]
    public async Task CreateLeave_ValidRequest_ShouldReturnSuccess()
    {
        var db = CreateDb();
        var service = new LeaveRequestService(db);

        var dto = new CreateLeaveRequestDto
        {
            FromDate = "2025-10-01",
            ToDate = "2025-10-03",
            Reason = "Personal work"
        };

        var (success, message) = await service.CreateAsync(1, dto);

        Assert.True(success);
        Assert.Equal("Leave request submitted.", message);
    }

    [Fact]
    public async Task CreateLeave_EmployeeNotFound_ShouldReturnFalse()
    {
        var db = CreateDb();
        var service = new LeaveRequestService(db);

        var dto = new CreateLeaveRequestDto
        {
            FromDate = "2025-10-01",
            ToDate = "2025-10-03",
            Reason = "Medical"
        };

        var (success, message) = await service.CreateAsync(999, dto);

        Assert.False(success);
        Assert.Equal("Employee not found.", message);
    }

    [Fact]
    public async Task CreateLeave_OverlappingDates_ShouldReturnFalse()
    {
        var db = CreateDb();
        var service = new LeaveRequestService(db);

        await service.CreateAsync(1, new CreateLeaveRequestDto
        {
            FromDate = "2025-10-01",
            ToDate = "2025-10-05",
            Reason = "Vacation"
        });

        var (success, message) = await service.CreateAsync(1, new CreateLeaveRequestDto
        {
            FromDate = "2025-10-03",
            ToDate = "2025-10-07",
            Reason = "Another trip"
        });

        Assert.False(success);
        Assert.Equal("You already have a leave request for these dates.", message);
    }
}
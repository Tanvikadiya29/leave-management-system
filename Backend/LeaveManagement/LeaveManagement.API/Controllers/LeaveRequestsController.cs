using LeaveManagement.API.DTOs;
using LeaveManagement.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LeaveManagement.API.Controllers;

[ApiController]
[Route("api/leave-requests")]
[Authorize]
public class LeaveRequestsController(LeaveRequestService leaveService) : ControllerBase
{
    private readonly LeaveRequestService _leaveService = leaveService;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var role = User.FindFirstValue(ClaimTypes.Role);
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            if (role == "Admin")
            {
                var all = await _leaveService.GetAllAsync();
                return Ok(all);
            }

            var mine = await _leaveService.GetByEmployeeAsync(userId);
            return Ok(mine);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Could not fetch leave requests.", error = ex.Message });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Employee")]
    public async Task<IActionResult> Create([FromBody] CreateLeaveRequestDto dto)
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var (success, message) = await _leaveService.CreateAsync(userId, dto);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Could not submit leave request.", error = ex.Message });
        }
    }

    [HttpPut("{id}/review")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Review(int id, [FromBody] ReviewLeaveDto dto)
    {
        try
        {
            var adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var (success, message) = await _leaveService.ReviewAsync(id, adminId, dto);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Could not review leave request.", error = ex.Message });
        }
    }
}
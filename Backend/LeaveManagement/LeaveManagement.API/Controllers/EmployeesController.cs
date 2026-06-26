using LeaveManagement.API.DTOs;
using LeaveManagement.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagement.API.Controllers;

[ApiController]
[Route("api/employees")]
[Authorize]
public class EmployeesController(EmployeeService employeeService) : ControllerBase
{
    private readonly EmployeeService _employeeService = employeeService;

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var list = await _employeeService.GetAllAsync();
            return Ok(list);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Could not fetch employees.", error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var emp = await _employeeService.GetByIdAsync(id);

            if (emp == null)
                return NotFound(new { message = "Employee not found." });

            return Ok(emp);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Could not fetch employee.", error = ex.Message });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] RegisterDto dto)
    {
        try
        {
            var (success, message) = await _employeeService.CreateAsync(dto);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Could not create employee.", error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var (success, message) = await _employeeService.DeleteAsync(id);

            if (!success)
                return NotFound(new { message });

            return Ok(new { message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Could not delete employee.", error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEmployeeDto dto)
    {
        try
        {
            var (success, message) = await _employeeService.UpdateAsync(id, dto);

            if (!success)
                return NotFound(new { message });

            return Ok(new { message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Could not update employee.", error = ex.Message });
        }
    }
}
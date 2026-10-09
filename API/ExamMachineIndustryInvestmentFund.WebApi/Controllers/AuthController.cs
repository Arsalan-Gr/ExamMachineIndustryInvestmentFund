using ExamMachineIndustryInvestmentFund.Application.DTOs.Auth;
using ExamMachineIndustryInvestmentFund.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExamMachineIndustryInvestmentFund.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        try
        {
            await authService.RegisterAsync(dto);

            return StatusCode(
                StatusCodes.Status201Created,
                new { Message = "User registered successfully." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var result = await authService.LoginAsync(dto);

        if (result is null)
            return Unauthorized(new
            {
                Message = "Invalid email or password."
            });

        return Ok(result);
    }



    [HttpGet("me")]
    [Authorize]
    public IActionResult GetCurrentUser()
    {
        return Ok(new
        {
            UserId = User.FindFirst(
                ClaimTypes.NameIdentifier)?.Value,
            Email = User.FindFirst(
                ClaimTypes.Email)?.Value,
            Roles = User.FindAll(ClaimTypes.Role)
                .Select(c => c.Value)
        });
    }

}
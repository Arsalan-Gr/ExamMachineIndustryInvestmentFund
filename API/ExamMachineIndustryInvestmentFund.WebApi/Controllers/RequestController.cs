
using System.Security.Claims;
using ExamMachineIndustryInvestmentFund.Application.DTOs.Requests;
using ExamMachineIndustryInvestmentFund.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExamMachineIndustryInvestmentFund.WebApi.Controllers;

[ApiController]
[Route("api/requests")]
[Authorize]
public class RequestController : ControllerBase
{
    private readonly IRequestService _requestService;

    public RequestController(IRequestService requestService)
    {
        _requestService = requestService;
    }

    [HttpPost]
    [Authorize(Roles = "Employee")]
    public async Task<ActionResult<RequestDto>> Create(
        [FromBody] CreateRequestDto dto,
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var result = await _requestService.CreateAsync(
            dto,
            userId,
            cancellationToken);

        return Created($"/api/requests/{result.Id}", result);
    }

    [HttpGet]
    [Authorize(Roles = "Employee,Manager,Finance")]
    public async Task<ActionResult<PagedResultDto<RequestDto>>> GetFiltered(
        [FromQuery] RequestFilterDto filter,
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var userRole =
            User.IsInRole("Employee")
                ? "Employee"
                : User.IsInRole("Manager")
                    ? "Manager"
                    : User.IsInRole("Finance")
                        ? "Finance"
                        : null;

        if (string.IsNullOrWhiteSpace(userId) ||
            string.IsNullOrWhiteSpace(userRole))
        {
            return Unauthorized();
        }

        var result = await _requestService.GetFilteredAsync(
            filter,
            userId,
            userRole,
            cancellationToken);

        return Ok(result);
    }

    [HttpPost("{id:guid}/approve")]
    [Authorize(Roles = "Manager,Finance")]
    public async Task<IActionResult> Approve(Guid id)
    {
        var reviewerUserId =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(reviewerUserId))
            return Unauthorized();

        var reviewerRoles = User.FindAll(ClaimTypes.Role)
            .Select(claim => claim.Value);

        await _requestService.ApproveRequestAsync(
            id,
            reviewerUserId,
            reviewerRoles);

        return NoContent();
    }

    [HttpPost("{id:guid}/reject")]
    [Authorize(Roles = "Manager,Finance")]
    public async Task<IActionResult> Reject(Guid id)
    {
        var reviewerUserId =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(reviewerUserId))
            return Unauthorized();

        var reviewerRoles = User.FindAll(ClaimTypes.Role)
            .Select(claim => claim.Value);

        await _requestService.RejectRequestAsync(
            id,
            reviewerUserId,
            reviewerRoles);

        return NoContent();
    }
}

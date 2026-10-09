using ExamMachineIndustryInvestmentFund.Application.DTOs.Auth;
using ExamMachineIndustryInvestmentFund.Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ExamMachineIndustryInvestmentFund.Core.Constants;
using ExamMachineIndustryInvestmentFund.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ExamMachineIndustryInvestmentFund.Infrastructure.Services;

public class AuthService
    (
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration,
        ApplicationDbContext dbContext
        ) : IAuthService
{
    public async Task RegisterAsync(RegisterDto dto)
    {
        const string EmployeeRole = "Employee";

        var existingUser = await userManager.FindByEmailAsync(dto.Email);

        if (existingUser is not null)
            throw new InvalidOperationException("Email is already registered.");

        if (!await roleManager.RoleExistsAsync(EmployeeRole))
            throw new InvalidOperationException("Employee role does not exist.");

        var user = new IdentityUser
        {
            UserName = dto.Email,
            Email = dto.Email
        };

        var result = await userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));

            throw new InvalidOperationException(errors);
        }

        var roleResult = await userManager.AddToRoleAsync(user, EmployeeRole);

        if (!roleResult.Succeeded)
        {
            await userManager.DeleteAsync(user);
            var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));

            throw new InvalidOperationException(errors);
        }
    }


    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        var user = await userManager.FindByEmailAsync(dto.Email);

        if (user is null ||
            !await userManager.CheckPasswordAsync(user, dto.Password))
            return null;

        var roles = await userManager.GetRolesAsync(user);
        if (roles.Count != 1)
            throw new InvalidOperationException(
                "User must have exactly one assigned role.");

        var allowedRoles = new[] { "Employee", "Manager", "Finance" };

        if (!allowedRoles.Contains(roles[0]))
            throw new InvalidOperationException(
                "User has an invalid role.");

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        claims.Add(new Claim(ClaimTypes.Role, roles[0]));

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));

        var credentials = new SigningCredentials(
            key, SecurityAlgorithms.HmacSha256);

        var expiresAt = DateTime.UtcNow.AddMinutes(
            int.Parse(configuration["Jwt:ExpirationMinutes"]!));

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials
        );

        return new AuthResponseDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAt = expiresAt,
            Email = user.Email!,
            Roles = roles
        };
    }


    public async Task ChangeUserRoleAsync(
        string userId,
        string newRole,
        CancellationToken cancellationToken = default)
    {
        string[] allowedRoles =
        [
            AppRoles.Employee,
            AppRoles.Manager,
            AppRoles.Finance
        ];

        if (!allowedRoles.Contains(newRole))
            throw new ArgumentException("Invalid role.");

        var user = await userManager.FindByIdAsync(userId)
                   ?? throw new KeyNotFoundException("User not found.");

        if (!await roleManager.RoleExistsAsync(newRole))
            throw new InvalidOperationException("Role does not exist.");

        await using var transaction =
            await dbContext.Database.BeginTransactionAsync(cancellationToken);

        var currentRoles = await userManager.GetRolesAsync(user);

        if (currentRoles.Count == 1 && currentRoles[0] == newRole)
            return;

        if (currentRoles.Count > 0)
        {
            var removeResult = await userManager.RemoveFromRolesAsync(
                user, currentRoles);

            if (!removeResult.Succeeded)
                throw new InvalidOperationException(
                    string.Join("; ", removeResult.Errors.Select(e => e.Description)));
        }

        var addResult = await userManager.AddToRoleAsync(user, newRole);

        if (!addResult.Succeeded)
            throw new InvalidOperationException(
                string.Join("; ", addResult.Errors.Select(e => e.Description)));

        await transaction.CommitAsync(cancellationToken);
    }


}
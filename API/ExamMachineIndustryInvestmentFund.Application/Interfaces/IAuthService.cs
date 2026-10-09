using ExamMachineIndustryInvestmentFund.Application.DTOs.Auth;

namespace ExamMachineIndustryInvestmentFund.Application.Interfaces;

public interface IAuthService
{
    Task RegisterAsync(RegisterDto registerDto);

    Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);

    Task ChangeUserRoleAsync(string userId, string newRole, CancellationToken cancelationToken);

}
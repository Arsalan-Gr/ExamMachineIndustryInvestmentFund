using System;
using System.Collections.Generic;
using System.Text;
using ExamMachineIndustryInvestmentFund.Core.Enums;

namespace ExamMachineIndustryInvestmentFund.Application.Interfaces.Identity
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
        bool IsAuthenticated { get; }
        bool IsInRole(UserRole role);
    }
}

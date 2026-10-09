using ExamMachineIndustryInvestmentFund.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExamMachineIndustryInvestmentFund.Application.Interfaces.Services
{ 
    public interface IRequestRoutingService 
    { 
        ApprovalRole DetermineApprovalRole(decimal amount);
    }
    
}

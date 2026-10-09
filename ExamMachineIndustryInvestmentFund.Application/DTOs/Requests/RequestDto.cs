using ExamMachineIndustryInvestmentFund.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExamMachineIndustryInvestmentFund.Application.DTOs.Requests
{
    public class RequestDto
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public RequestStatus Status { get; set; }

        public ApprovalRole AssignedRole { get; set; }

        public string CreatedByUserId { get; set; } = string.Empty;

        public string? ReviewedByUserId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ReviewedAt { get; set; }
    }
}

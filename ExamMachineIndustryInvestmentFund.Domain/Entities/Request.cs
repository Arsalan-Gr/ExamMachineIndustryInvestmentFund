using System;
using System.Collections.Generic;
using System.Text;
using ExamMachineIndustryInvestmentFund.Core.Enums;

namespace ExamMachineIndustryInvestmentFund.Core.Entities
{
    public class Request
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public RequestStatus Status { get; set; } = RequestStatus.Created;

        public ApprovalRole AssignedRole { get; set; }

        public string CreatedByUserId { get; set; } = string.Empty;

        public string? ReviewedByUserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ReviewedAt { get; set; }

    }
}

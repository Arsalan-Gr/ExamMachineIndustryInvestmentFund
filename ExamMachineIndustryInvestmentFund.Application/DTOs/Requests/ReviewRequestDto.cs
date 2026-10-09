using ExamMachineIndustryInvestmentFund.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ExamMachineIndustryInvestmentFund.Application.DTOs.Requests
{
    public class ReviewRequestDto
    {
        [Required]
        public Guid RequestId { get; set; }

        [Required]
        public RequestStatus Decision { get; set; }
    }
}

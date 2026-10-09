using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using ExamMachineIndustryInvestmentFund.Application.Validators;

namespace ExamMachineIndustryInvestmentFund.Application.DTOs.Requests
{
    public class CreateRequestDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        [PositiveDecimal]
        public decimal Amount { get; set; }
    }
}

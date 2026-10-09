using ExamMachineIndustryInvestmentFund.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using ExamMachineIndustryInvestmentFund.Application.Validators;

namespace ExamMachineIndustryInvestmentFund.Application.DTOs.Requests
{
    public class RequestFilterDto : IValidatableObject
    {
        public RequestStatus? Status { get; set; }

        public ApprovalRole? AssignedRole { get; set; }

        public DateTime? CreatedFrom { get; set; }

        public DateTime? CreatedTo { get; set; }

        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; } = 1;

        [Range(1, 100)]
        public int PageSize { get; set; } = 10;

        [PositiveDecimal]
        public decimal? MinAmount { get; set; }

        [PositiveDecimal]
        public decimal? MaxAmount { get; set; }

        public string? SearchTerm { get; set; }

        public string SortBy { get; set; } = "CreatedAt";

        public bool SortDescending { get; set; } = true;



        public IEnumerable<ValidationResult> Validate(
            ValidationContext validationContext)
        {
            if (MinAmount.HasValue &&
                MaxAmount.HasValue &&
                MinAmount > MaxAmount)
            {
                yield return new ValidationResult(
                    "MinAmount cannot be greater than MaxAmount.",
                    [nameof(MinAmount), nameof(MaxAmount)]);
            }

            if (CreatedFrom.HasValue &&
                CreatedTo.HasValue &&
                CreatedFrom > CreatedTo)
            {
                yield return new ValidationResult(
                    "CreatedFrom cannot be later than CreatedTo.",
                    [nameof(CreatedFrom), nameof(CreatedTo)]);
            }
        }
    }
}

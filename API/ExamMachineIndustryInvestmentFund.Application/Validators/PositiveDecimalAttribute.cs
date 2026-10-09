using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ExamMachineIndustryInvestmentFund.Application.Validators
{
    public sealed class PositiveDecimalAttribute: ValidationAttribute
    {
        public PositiveDecimalAttribute()
        {
            ErrorMessage = "Amount must be greater than zero.";
        }


        protected override ValidationResult? IsValid(
            object? value,
            ValidationContext validationContext)
        {
            if (value is null)
                return ValidationResult.Success;

            if (value is decimal amount && amount > 0)
                return ValidationResult.Success;

            return new ValidationResult(
                $"{validationContext.DisplayName} must be greater than zero.");
        }
    }
}



using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ExamMachineIndustryInvestmentFund.Core.Enums
{
    public enum UserRole
    {
        [Display(Name = "نامشخص", Description = "نقش مشخص نیست.")]
        Unknown = 0,

        [Display(Name = "کارمند", Description = "کارمند مجموعه")]
        Employee = 1,

        [Display(Name = "امور مالی", Description = "عضو امور مالی مجموعه")]
        Finance = 2,

        [Display(Name = "مدیر", Description = "مدیریت مجموعه")]
        Manager = 3,
    }
}

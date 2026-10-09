using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ExamMachineIndustryInvestmentFund.Core.Enums
{
    // نقش تأیید کننده
    public enum ApprovalRole
    {
        [Display(Name = "نامشخص", Description = "نقش تأیید کننده مشخص نیست.")]
        Unknown = 0,


        [Display(Name = "امور مالی", Description = "تأیید کننده از امور مالی مجموعه")]
        Finance = 1,

        [Display(Name = "مدیر", Description = "تأیید کننده از مدیریت مجموعه")]
        Manager = 2,
    }
}

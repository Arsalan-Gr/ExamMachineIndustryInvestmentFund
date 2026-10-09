using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ExamMachineIndustryInvestmentFund.Core.Enums
{
    public enum RequestStatus
    {
        [Display(Name = "نامشخص", Description = "وضعیت درخواست مشخص نیست.")]
        Unknown = 0,

        [Display(Name = "ایجاد", Description = "درخواست تازه ایجاد شده")]
        Created = 1,

        [Display(Name = "در حال بررسی", Description = "پس از ایجاد درخواست برای انجام مراحل بررسی")]
        Pending = 2,

        [Display(Name = "پذیرفته شده", Description = "تأیید درخواست")]
        Approved = 3,

        [Display(Name = "رد شده", Description = "رد درخواست")]
        Rejected = 4,
    }
}

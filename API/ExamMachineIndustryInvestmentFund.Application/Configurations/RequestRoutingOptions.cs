using System;
using System.Collections.Generic;
using System.Text;

namespace ExamMachineIndustryInvestmentFund.Application.Configurations
{
    public class RequestRoutingOptions
    {
        public const string SectionName = "RequestRouting";
        public decimal ApprovalThreshold { get; set; }
    }
}

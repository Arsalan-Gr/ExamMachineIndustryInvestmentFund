using System;
using System.Collections.Generic;
using System.Text;
using ExamMachineIndustryInvestmentFund.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace ExamMachineIndustryInvestmentFund.Infrastructure.Identity
{
    public static class IdentityConfiguration
    {
        public static IServiceCollection AddIdentityConfigration(
            this IServiceCollection services)
        {
            services.AddIdentity<IdentityUser, IdentityRole>(options =>
                {
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequiredLength = 8;

                    options.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            
            return services;
        }
    }
}

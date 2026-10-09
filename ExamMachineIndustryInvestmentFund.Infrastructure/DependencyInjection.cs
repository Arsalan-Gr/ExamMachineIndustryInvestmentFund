using System;
using System.Collections.Generic;
using System.Text;
using ExamMachineIndustryInvestmentFund.Infrastructure.Identity;
using ExamMachineIndustryInvestmentFund.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExamMachineIndustryInvestmentFund.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = configuration
                                       .GetConnectionString("DefaultConnection")
                                   ?? throw new InvalidOperationException(
                                       "DefaultConnection is not configured.");

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });

            services.AddIdentityConfigration();

            return services;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using ExamMachineIndustryInvestmentFund.Core.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace ExamMachineIndustryInvestmentFund.Infrastructure.Identity
{
    public static class IdentitySeeder
    {

        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var roleManager = scope.ServiceProvider
                .GetRequiredService<RoleManager<IdentityRole>>();

            string[] roles = [AppRoles.Employee, AppRoles.Manager, AppRoles.Finance];

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var result = await roleManager.CreateAsync(
                        new IdentityRole(role));

                    if (!result.Succeeded)
                    {
                        throw new InvalidOperationException(
                            string.Join("; ",
                            result.Errors.Select(e => e.Description)));
                    }
                }
                
            }
        }

    }
}

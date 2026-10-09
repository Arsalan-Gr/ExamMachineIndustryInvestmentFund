using ExamMachineIndustryInvestmentFund.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExamMachineIndustryInvestmentFund.Infrastructure.Persistence.Configurations
{
    public class RequestConfiguration: IEntityTypeConfiguration<Request>
    {
        public void Configure(EntityTypeBuilder<Request> builder)
        {
            // table name 
            builder.ToTable("Requests");


            // keys
            builder.HasKey(r => r.Id);


            // properties
            builder.Property(r => r.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(r => r.Description)
                .HasColumnType("text");

            builder.Property(r => r.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(r => r.Status)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(r => r.AssignedRole)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(r => r.CreatedByUserId)
                .IsRequired();

            builder.Property(r => r.CreatedAt)
                .IsRequired();


            // relations
            builder.HasOne<IdentityUser>()
                .WithMany()
                .HasForeignKey(r => r.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<IdentityUser>()
                .WithMany()
                .HasForeignKey(r => r.ReviewedByUserId)
                .OnDelete(DeleteBehavior.Restrict);


            // non-clustered indexes 
            builder.HasIndex(r => r.CreatedByUserId);

            builder.HasIndex(r => new
            {
                r.AssignedRole,
                r.Status
            });

            builder.HasIndex(r => r.CreatedAt);
        }
    }
}

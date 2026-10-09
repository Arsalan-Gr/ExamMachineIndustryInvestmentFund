using System;
using System.Collections.Generic;
using System.Text;
using ExamMachineIndustryInvestmentFund.Application.DTOs.Requests;
using ExamMachineIndustryInvestmentFund.Core.Entities;
using ExamMachineIndustryInvestmentFund.Core.Enums;

namespace ExamMachineIndustryInvestmentFund.Application.Interfaces.Repositories
{
    public interface IRequestRepository
    {
        Task<Request?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            Request request,
            CancellationToken cancellationToken = default);

        Task UpdateAsync(
            Request request,
            CancellationToken cancellationToken = default);

        Task<PagedResultDto<Request>> GetPagedAsync(
            RequestFilterDto filter,
            string? createdByUserId,
            ApprovalRole? assignedRole,
            CancellationToken cancellationToken = default);

        Task<PagedResultDto<Request>> GetFilteredAsync(
            RequestFilterDto filter,
            string? createdByUserId = null,
            ApprovalRole? visibleRole = null,
            CancellationToken cancellationToken = default);

        Task<Request?> GetByIdAsync(Guid id);

        Task UpdateAsync(Request request);
    }
}

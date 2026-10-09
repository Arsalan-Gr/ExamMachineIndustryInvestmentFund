using System;
using System.Collections.Generic;
using System.Text;
using ExamMachineIndustryInvestmentFund.Application.DTOs.Requests;

namespace ExamMachineIndustryInvestmentFund.Application.Interfaces.Services
{
    public interface IRequestService
    {

        Task<RequestDto> CreateAsync(
            CreateRequestDto dto,
            string userId,
            CancellationToken cancellationToken = default);

        Task<RequestDto> ReviewAsync(
            ReviewRequestDto dto,
            CancellationToken cancellationToken = default);

        Task<RequestDto?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<PagedResultDto<RequestDto>> GetPagedAsync(
            RequestFilterDto filter,
            CancellationToken cancellationToken = default);


        Task<PagedResultDto<RequestDto>> GetFilteredAsync(
            RequestFilterDto filter,
            string currentUserId,
            string currentUserRole,
            CancellationToken cancellationToken = default);


        Task ApproveRequestAsync(Guid requestId, string reviewerUserId, IEnumerable<string> reviewerRoles);
        Task RejectRequestAsync(Guid requestId, string reviewerUserId, IEnumerable<string> reviewerRoles);


    }
}

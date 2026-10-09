using ExamMachineIndustryInvestmentFund.Application.Configurations;
using ExamMachineIndustryInvestmentFund.Application.DTOs.Requests;
using ExamMachineIndustryInvestmentFund.Application.Interfaces.Repositories;
using ExamMachineIndustryInvestmentFund.Core.Entities;
using ExamMachineIndustryInvestmentFund.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using ExamMachineIndustryInvestmentFund.Application.Interfaces.Services;
using Microsoft.Extensions.Options;

namespace ExamMachineIndustryInvestmentFund.Application.Services
{
    public class RequestService: IRequestService

    {
        private readonly IRequestRepository _requestRepository;
        private readonly RequestRoutingOptions _routingOptions;

        public RequestService(
            IRequestRepository requestRepository,
            IOptions<RequestRoutingOptions> routingOptions)
        {
            _requestRepository = requestRepository;
            _routingOptions = routingOptions.Value;
        }

        public async Task<RequestDto> CreateAsync(
            CreateRequestDto dto,
            string userId,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(userId);

            var assignedRole = dto.Amount <= _routingOptions.ApprovalThreshold
                ? ApprovalRole.Manager
                : ApprovalRole.Finance;

            var request = new Request
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                Amount = dto.Amount,
                Status = RequestStatus.Created,
                AssignedRole = assignedRole,
                CreatedByUserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _requestRepository.AddAsync(request, cancellationToken);

            return new RequestDto
            {
                Id = request.Id,
                Title = request.Title,
                Description = request.Description,
                Amount = request.Amount,
                Status = request.Status,
                AssignedRole = request.AssignedRole,
                CreatedByUserId = request.CreatedByUserId,
                ReviewedByUserId = request.ReviewedByUserId,
                CreatedAt = request.CreatedAt,
                ReviewedAt = request.ReviewedAt
            };
        }


        public Task<RequestDto> ReviewAsync(
            ReviewRequestDto dto,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<RequestDto?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResultDto<RequestDto>> GetPagedAsync(
            RequestFilterDto filter,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<PagedResultDto<RequestDto>> GetFilteredAsync(
            RequestFilterDto filter,
            string currentUserId,
            string currentUserRole,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(filter);

            if (string.IsNullOrWhiteSpace(currentUserId))
                throw new UnauthorizedAccessException();

            string? createdByUserId = null;
            ApprovalRole? visibleRole = null;
            switch (currentUserRole)
            {
                case "Employee":
                    createdByUserId = currentUserId;
                    break;
                case "Manager":
                    visibleRole = ApprovalRole.Manager;
                    break;
                case "Finance":
                    visibleRole = ApprovalRole.Finance;
                    break;

                default:
                    throw new UnauthorizedAccessException
                        ("You are not allowed to view requests.");
            }


            var result = await _requestRepository
                .GetFilteredAsync
                (
                    filter,
                    createdByUserId,
                    visibleRole,
                    cancellationToken
                );

            var items = result.Items.Select(request => new RequestDto
            {
                Id = request.Id,
                Title = request.Title,
                Description = request.Description,
                Amount = request.Amount,
                Status = request.Status,
                AssignedRole = request.AssignedRole,
                CreatedByUserId = request.CreatedByUserId,
                ReviewedByUserId = request.ReviewedByUserId,
                CreatedAt = request.CreatedAt,
                ReviewedAt = request.ReviewedAt
            }).ToList();

            return new PagedResultDto<RequestDto>
            {
                Items = items,
                TotalCount = result.TotalCount,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize
            };
        }



        public async Task ApproveRequestAsync(
            Guid requestId,
            string reviewerUserId,
            IEnumerable<string> reviewerRoles)
        {
            await ReviewRequestAsync(
                requestId,
                reviewerUserId,
                reviewerRoles,
                RequestStatus.Approved);
        }

        public async Task RejectRequestAsync(
            Guid requestId,
            string reviewerUserId,
            IEnumerable<string> reviewerRoles)
        {
            await ReviewRequestAsync(
                requestId,
                reviewerUserId,
                reviewerRoles,
                RequestStatus.Rejected);
        }

        private async Task ReviewRequestAsync(
            Guid requestId,
            string reviewerUserId,
            IEnumerable<string> reviewerRoles,
            RequestStatus decision)
        {
            if (requestId == Guid.Empty)
                throw new ArgumentException("Invalid request ID.");

            if (string.IsNullOrWhiteSpace(reviewerUserId))
                throw new ArgumentException("Invalid reviewer user ID.");

            if (decision is not (RequestStatus.Approved or RequestStatus.Rejected))
                throw new ArgumentException("Invalid review decision.");

            var request = await _requestRepository.GetByIdAsync(requestId);

            if (request is null)
                throw new KeyNotFoundException("Request not found.");

            if (request.Status is not
                (RequestStatus.Created or RequestStatus.Pending))
            {
                throw new InvalidOperationException(
                    "This request has already been reviewed.");
            }

            var requiredRole = request.AssignedRole.ToString();

            if (reviewerRoles is null ||
                !reviewerRoles.Contains(
                    requiredRole,
                    StringComparer.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException(
                    $"Only users with the '{requiredRole}' role can review this request.");
            }

            request.Status = decision;
            request.ReviewedByUserId = reviewerUserId;
            request.ReviewedAt = DateTime.UtcNow;

            await _requestRepository.UpdateAsync(request);
        }




    }


}

using ExamMachineIndustryInvestmentFund.Application.DTOs.Requests;
using ExamMachineIndustryInvestmentFund.Application.Interfaces.Repositories;
using ExamMachineIndustryInvestmentFund.Core.Entities;
using ExamMachineIndustryInvestmentFund.Core.Enums;
using ExamMachineIndustryInvestmentFund.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ExamMachineIndustryInvestmentFund.Infrastructure.Repositories;

public class RequestRepository : IRequestRepository
{
    private readonly ApplicationDbContext _context;

    public RequestRepository(ApplicationDbContext context)
        => _context = context;


    public async Task<Request?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Requests
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task AddAsync(
        Request request,
        CancellationToken cancellationToken = default)
    {
        await _context.Requests.AddAsync(request, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(
        Request request,
        CancellationToken cancellationToken = default)
    {
        _context.Requests.Update(request);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<PagedResultDto<Request>> GetPagedAsync(
        RequestFilterDto filter,
        string? createdByUserId,
        ApprovalRole? assignedRole,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Requests.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(createdByUserId))
            query = query.Where(x => x.CreatedByUserId == createdByUserId);

        if (assignedRole.HasValue)
            query = query.Where(x => x.AssignedRole == assignedRole.Value);

        // Additional filters will be implemented in Step 7.

        var totalCount = await query.CountAsync(cancellationToken);

        var pageNumber = Math.Max(1, filter.PageNumber);
        var pageSize = Math.Clamp(filter.PageSize, 1, 100);

        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .ThenByDescending(x => x.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResultDto<Request>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }


    
    public async Task<PagedResultDto<Request>> GetFilteredAsync(
        RequestFilterDto filter,
        string? createdByUserId = null,
        ApprovalRole? visibleRole = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Request> query = _context.Requests
            .AsNoTracking();

        if (createdByUserId is not null)
            query = query.Where(r => r.CreatedByUserId == createdByUserId);

        if (visibleRole.HasValue)
            query = query.Where(r => r.AssignedRole == visibleRole.Value);

        // Status filtering
        if (filter.Status.HasValue)
        {
            query = query.Where(r =>
                r.Status == filter.Status.Value);
        }

        // Assigned role filtering
        if (filter.AssignedRole.HasValue)
        {
            query = query.Where(r =>
                r.AssignedRole == filter.AssignedRole.Value);
        }

        // Date filtering
        if (filter.CreatedFrom.HasValue)
        {
            query = query.Where(r =>
                r.CreatedAt >= filter.CreatedFrom.Value);
        }

        if (filter.CreatedTo.HasValue)
        {
            query = query.Where(r =>
                r.CreatedAt <= filter.CreatedTo.Value);
        }

        // Amount filtering
        if (filter.MinAmount.HasValue)
        {
            query = query.Where(r =>
                r.Amount >= filter.MinAmount.Value);
        }

        if (filter.MaxAmount.HasValue)
        {
            query = query.Where(r =>
                r.Amount <= filter.MaxAmount.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var searchTerm = filter.SearchTerm.Trim();

            query = query.Where
                (
                    r=>
                        EF.Functions.ILike(r.Title, $"%{searchTerm}%") ||
                        EF.Functions.ILike(r.Description, $"%{searchTerm}%")
                );
        }

        // Total matching records before pagination
        int totalCount = await query.CountAsync(cancellationToken);

        // Sorting and pagination
        // Sorting
        IOrderedQueryable<Request> orderedQuery =
            (filter.SortBy.ToLowerInvariant(), filter.SortDescending) switch
            {
                ("title", false) => query.OrderBy(r => r.Title),
                ("title", true) => query.OrderByDescending(r => r.Title),

                ("amount", false) => query.OrderBy(r => r.Amount),
                ("amount", true) => query.OrderByDescending(r => r.Amount),

                ("status", false) => query.OrderBy(r => r.Status),
                ("status", true) => query.OrderByDescending(r => r.Status),

                ("createdat", false) => query.OrderBy(r => r.CreatedAt),
                _ => query.OrderByDescending(r => r.CreatedAt)
            };

        // Pagination
        List<Request> items = await orderedQuery
            .ThenBy(r => r.Id)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken);


        return new PagedResultDto<Request>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }


    public async Task<Request?> GetByIdAsync(Guid id)
    {
        return await _context.Requests
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task UpdateAsync(Request request)
    {
        _context.Requests.Update(request);
        await _context.SaveChangesAsync();
    }

}
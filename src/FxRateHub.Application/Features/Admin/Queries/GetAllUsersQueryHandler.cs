using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FxRateHub.Application.Common.Interfaces;
using FxRateHub.Application.Common.Models;
using FxRateHub.Application.Features.Admin.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FxRateHub.Application.Features.Admin.Queries;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, PaginatedList<AdminUserDto>>
{
    private readonly IApplicationDbContext _context;
    
    public GetAllUsersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<PaginatedList<AdminUserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Users
            .Include(u => u.ApiKey)
            .OrderByDescending(u => u.CreatedAt)
            .AsQueryable();
        
        // Apply search filter
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchLower = request.Search.ToLower();
            query = query.Where(u => 
                u.Email.ToLower().Contains(searchLower) || 
                u.FullName.ToLower().Contains(searchLower));
        }
        
        // Materialize the query to handle the null check
        var users = await query.ToListAsync(cancellationToken);
        
        var resultList = users.Select(u => new AdminUserDto(
            u.Id,
            u.Email,
            u.FullName,
            u.Role.ToString(),
            u.AuthProvider.ToString(),
            u.IsBlocked,
            u.ApiKey != null,
            null,
            null,
            u.CreatedAt,
            u.ApiKey?.LastRequestDate
        )).AsQueryable();
        
        return await PaginatedList<AdminUserDto>.CreateAsync(resultList, request.Page, request.PageSize);
    }
}

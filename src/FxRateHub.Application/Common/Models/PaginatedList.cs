using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FxRateHub.Application.Common.Models;

/// <summary>
/// Generic class for paginated results
/// </summary>
/// <typeparam name="T">The type of items in the paginated list</typeparam>
public class PaginatedList<T>
{
    public List<T> Items { get; private set; } = new();
    public int PageNumber { get; private set; }
    public int TotalPages { get; private set; }
    public int TotalCount { get; private set; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    private PaginatedList() { }

    public static Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
    {
        var totalCount = source.Count();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Task.FromResult(new PaginatedList<T>
        {
            Items = items,
            PageNumber = pageNumber,
            TotalPages = totalPages,
            TotalCount = totalCount
        });
    }
}

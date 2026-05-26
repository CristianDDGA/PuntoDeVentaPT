namespace PuntoVenta.Application.DTOs.Common;

/// <summary>
/// Generic wrapper for paginated API responses.
/// </summary>
/// <typeparam name="TItem">Type of each item in the result set.</typeparam>
public sealed class PagedResult<TItem>
{
    public IReadOnlyList<TItem> Items      { get; init; } = [];
    public int                  TotalCount { get; init; }
    public int                  Page       { get; init; }
    public int                  PageSize   { get; init; }
    public int                  TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;

    public static PagedResult<TItem> Create(IEnumerable<TItem> items, int totalCount, int page, int pageSize)
        => new()
        {
            Items      = items.ToList().AsReadOnly(),
            TotalCount = totalCount,
            Page       = page,
            PageSize   = pageSize
        };
}

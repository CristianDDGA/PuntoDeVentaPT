namespace PuntoVenta.Blazor.Models;

/// <summary>
/// Client-side equivalent of the API's generic PagedResult DTO.
/// </summary>
/// <typeparam name="TItem">Type of each item in the result set.</typeparam>
public sealed class PagedResultModel<TItem>
{
    public List<TItem> Items      { get; set; } = [];
    public int         TotalCount { get; set; }
    public int         Page       { get; set; }
    public int         PageSize   { get; set; }
    public int         TotalPages { get; set; }
}

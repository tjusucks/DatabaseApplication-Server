namespace DbApp.Application.Common;

/// <summary>
/// Generic paginated result wrapper.
/// </summary>
public class PaginatedResult<T>
{
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public List<T> Items { get; set; } = [];
}

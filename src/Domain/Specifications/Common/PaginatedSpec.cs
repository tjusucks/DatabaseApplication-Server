namespace DbApp.Domain.Specifications.Common;

public class PaginatedSpec<T>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public bool Descending { get; set; }
    public string OrderBy { get; set; } = String.Empty;
    public T InnerSpec { get; set; } = default!;
}

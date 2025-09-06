namespace DbApp.Domain.Specifications.Common;

public class GroupedSpec<T>
{
    public bool Descending { get; set; }
    public string GroupBy { get; set; } = String.Empty;
    public string OrderBy { get; set; } = String.Empty;
    public T InnerSpec { get; set; } = default!;
}

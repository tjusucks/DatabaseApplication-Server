namespace DbApp.Application.Common;

/// <summary>
/// Generic grouped result wrapper.
/// </summary>
public class GroupedResult<T>
{
    public string GroupKey { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public T Item { get; set; } = default!;
}

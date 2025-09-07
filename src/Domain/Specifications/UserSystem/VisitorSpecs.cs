using DbApp.Domain.Enums.UserSystem;

namespace DbApp.Domain.Specifications.UserSystem;

/// <summary>
/// Specification for searching visitors with user and visitor-specific criteria.
/// </summary>
public class VisitorSpec : UserSpec
{
    public VisitorType? VisitorType { get; set; }
    public int? PointsMin { get; set; }
    public int? PointsMax { get; set; }
    public string? MemberLevel { get; set; }
    public DateTime? MemberSinceStart { get; set; }
    public DateTime? MemberSinceEnd { get; set; }
    public bool? IsBlacklisted { get; set; }
    public int? HeightMin { get; set; }
    public int? HeightMax { get; set; }
}

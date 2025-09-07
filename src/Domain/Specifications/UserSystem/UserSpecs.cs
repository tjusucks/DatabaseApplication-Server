using DbApp.Domain.Enums.UserSystem;

namespace DbApp.Domain.Specifications.UserSystem;

/// <summary>
/// Specification for searching users with multiple criteria.
/// </summary>
public class UserSpec
{
    public string? Keyword { get; set; } // Search keyword in username, email, display name, and phone number.
    public int? UserId { get; set; }
    public string? Email { get; set; } // Exact email match
    public string? PhoneNumber { get; set; } // Exact phone number match
    public DateTime? BirthDateStart { get; set; }
    public DateTime? BirthDateEnd { get; set; }
    public Gender? Gender { get; set; }
    public DateTime? RegisterTimeStart { get; set; }
    public DateTime? RegisterTimeEnd { get; set; }
    public int? PermissionLevelMin { get; set; }
    public int? PermissionLevelMax { get; set; }
    public int? RoleId { get; set; }
    public DateTime? CreatedAtStart { get; set; }
    public DateTime? CreatedAtEnd { get; set; }
    public DateTime? UpdatedAtStart { get; set; }
    public DateTime? UpdatedAtEnd { get; set; }
}

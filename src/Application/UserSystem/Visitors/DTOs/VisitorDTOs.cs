using System.ComponentModel.DataAnnotations;
using DbApp.Domain.Enums.UserSystem;

namespace DbApp.Application.UserSystem.Visitors.DTOs;

/// <summary>
/// Data transfer object for visitor registration.
/// </summary>
public class RegisterVisitorDto
{
    /// <summary>
    /// The user ID to create visitor for.
    /// </summary>
    [Required]
    public int UserId { get; set; }

    /// <summary>
    /// The visitor's height in centimeters.
    /// </summary>
    [Required]
    [Range(50, 300, ErrorMessage = "Height must be between 50 and 300 cm")]
    public int Height { get; set; }
}

/// <summary>
/// Data transfer object for adding points.
/// </summary>
public class AddPointsDto
{
    /// <summary>
    /// The visitor ID.
    /// </summary>
    [Required]
    public int VisitorId { get; set; }

    /// <summary>
    /// The points to add.
    /// </summary>
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Points must be positive")]
    public int Points { get; set; }

    /// <summary>
    /// The reason for adding points.
    /// </summary>
    [Required]
    [StringLength(200, ErrorMessage = "Reason cannot exceed 200 characters")]
    public string Reason { get; set; } = string.Empty;
}

/// <summary>
/// Data transfer object for deducting points.
/// </summary>
public class DeductPointsDto
{
    /// <summary>
    /// The visitor ID.
    /// </summary>
    [Required]
    public int VisitorId { get; set; }

    /// <summary>
    /// The points to deduct.
    /// </summary>
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Points must be positive")]
    public int Points { get; set; }

    /// <summary>
    /// The reason for deducting points.
    /// </summary>
    [Required]
    [StringLength(200, ErrorMessage = "Reason cannot exceed 200 characters")]
    public string Reason { get; set; } = string.Empty;
}

/// <summary>
/// Data transfer object for updating visitor information.
/// </summary>
public class UpdateVisitorDto
{
    /// <summary>
    /// The visitor's height in centimeters.
    /// </summary>
    [Required]
    [Range(50, 300, ErrorMessage = "Height must be between 50 and 300 cm")]
    public int Height { get; set; }
}

/// <summary>
/// Data transfer object for blacklisting a visitor.
/// </summary>
public class BlacklistVisitorDto
{
    /// <summary>
    /// The reason for blacklisting.
    /// </summary>
    [Required]
    [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters")]
    public string Reason { get; set; } = string.Empty;
}

/// <summary>
/// Response model for visitor information.
/// </summary>
public class VisitorResponseDto
{
    public int VisitorId { get; set; }
    public VisitorType VisitorType { get; set; }
    public int Points { get; set; }
    public string? MemberLevel { get; set; }
    public DateTime? MemberSince { get; set; }
    public bool IsBlacklisted { get; set; }
    public int Height { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // User information
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public DateTime? BirthDate { get; set; }
    public Gender? Gender { get; set; }
}

/// <summary>
/// Response model for points operation result.
/// </summary>
public class PointsOperationResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int CurrentPoints { get; set; }
    public string? CurrentMemberLevel { get; set; }
    public bool LevelChanged { get; set; }
    public string? NewLevel { get; set; }
}

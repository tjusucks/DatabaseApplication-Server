using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Enums.UserSystem;

namespace DbApp.Domain.Entities.UserSystem;

/// <summary>
/// Staff team entity representing work groups within the theme park.
/// </summary>
public class StaffTeam
{
    /// <summary>
    /// Team unique identifier.
    /// </summary>
    public int TeamId { get; set; }

    /// <summary>
    /// Name of the work team.
    /// </summary>
    public string TeamName { get; set; } = string.Empty;

    /// <summary>
    /// Type of team based on specialization (Inspector, Mechanic, Mixed).
    /// </summary>
    public TeamType TeamType { get; set; }

    /// <summary>
    /// Foreign key reference to the team leader.
    /// </summary>
    public int LeaderId { get; set; }

    /// <summary>
    /// Creation timestamp.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Last update timestamp.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties.
    public Employee Leader { get; set; } = null!;
    public ICollection<TeamMember> TeamMembers { get; set; } = [];
    public ICollection<Employee> Employees { get; set; } = [];
    public ICollection<InspectionRecord> InspectionRecords { get; set; } = [];
    public ICollection<MaintenanceRecord> MaintenanceRecords { get; set; } = [];
}

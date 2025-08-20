namespace DbApp.Domain.Entities.UserSystem;

/// <summary>
/// Team member entity representing the many-to-many relationship between employees and teams.
/// </summary>
public class TeamMember
{
    /// <summary>
    /// Foreign key reference to the team.
    /// </summary>
    public int TeamId { get; set; }

    /// <summary>
    /// Foreign key reference to the employee.
    /// </summary>
    public int EmployeeId { get; set; }

    /// <summary>
    /// Date when the employee joined the team.
    /// </summary>
    public DateTime JoinDate { get; set; } = DateTime.UtcNow;

    // Navigation properties.
    public StaffTeam Team { get; set; } = null!;
    public Employee Employee { get; set; } = null!;
}

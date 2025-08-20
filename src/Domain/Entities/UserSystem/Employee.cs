using DbApp.Domain.Enums.UserSystem;

namespace DbApp.Domain.Entities.UserSystem;

/// <summary>
/// Employee entity containing staff information and organizational details.
/// Extends user information for theme park employees.
/// </summary>
public class Employee
{
    /// <summary>
    /// Employee ID, which is also a foreign key to the User entity.
    /// </summary>
    public int EmployeeId { get; set; }

    /// <summary>
    /// Unique employee identification number.
    /// </summary>
    public string StaffNumber { get; set; } = string.Empty;

    /// <summary>
    /// Employee's job position.
    /// </summary>
    public string Position { get; set; } = string.Empty;

    /// <summary>
    /// Department name where the employee works.
    /// </summary>
    public string? DepartmentName { get; set; }

    /// <summary>
    /// Type of staff (Regular, Inspector, Mechanic, Manager).
    /// </summary>
    public StaffType? StaffType { get; set; }

    /// <summary>
    /// Foreign key reference to the employee's primary team.
    /// </summary>
    public int? TeamId { get; set; }

    /// <summary>
    /// Date when the employee was hired.
    /// </summary>
    public DateTime HireDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Current employment status (Active, Resigned, OnLeave).
    /// </summary>
    public EmploymentStatus EmploymentStatus { get; set; } = EmploymentStatus.Active;

    /// <summary>
    /// Foreign key reference to the employee's direct manager.
    /// </summary>
    public int? ManagerId { get; set; }

    /// <summary>
    /// Professional certifications and qualifications.
    /// </summary>
    public string? Certification { get; set; }

    /// <summary>
    /// Specific area or zone the employee is responsible for.
    /// </summary>
    public string? ResponsibilityArea { get; set; }

    /// <summary>
    /// Creation timestamp.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Last update timestamp.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties.
    public User User { get; set; } = null!;
    public Employee? Manager { get; set; }
    public StaffTeam? Team { get; set; }
    public ICollection<Employee> Subordinates { get; set; } = [];
}

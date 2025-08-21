namespace DbApp.Domain.Enums.UserSystem;

/// <summary>
/// Enum representing different employment statuses for employees.
/// </summary>
public enum EmploymentStatus
{
    /// <summary>
    /// Currently active employee.
    /// </summary>
    Active = 0,

    /// <summary>
    /// Employee has resigned.
    /// </summary>
    Resigned = 1,

    /// <summary>
    /// Employee is on leave.
    /// </summary>
    OnLeave = 2,
}

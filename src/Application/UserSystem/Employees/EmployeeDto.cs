using DbApp.Domain.Enums.UserSystem;

namespace DbApp.Application.UserSystem.Employees;

public class EmployeeDto
{
    public int EmployeeId { get; set; }
    public string StaffNumber { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string? DepartmentName { get; set; }
    public StaffType? StaffType { get; set; }
    public int? TeamId { get; set; }
    public DateTime HireDate { get; set; }
    public EmploymentStatus EmploymentStatus { get; set; }
    public int? ManagerId { get; set; }
    public string? Certification { get; set; }
    public string? ResponsibilityArea { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // 只包含简单信息的关联对象，避免循环引用
    public EmployeeSimpleDto? Manager { get; set; }
    public TeamSimpleDto? Team { get; set; }
    public UserSimpleDto User { get; set; } = new();
}

public class EmployeeSimpleDto
{
    public int EmployeeId { get; set; }
    public string StaffNumber { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string? DepartmentName { get; set; }
    public StaffType? StaffType { get; set; }
}

public class TeamSimpleDto
{
    public int TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public string TeamType { get; set; } = string.Empty;
}

public class UserSimpleDto
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}

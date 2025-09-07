using System.ComponentModel.DataAnnotations;
using DbApp.Domain.Enums.UserSystem;
using MediatR;

namespace DbApp.Application.UserSystem.Employees;

public enum EmployeeType
{
    Manager = 0,
    Employee = 1
}

public record CreateEmployeeCommand(
    [Required(ErrorMessage = "Username is required")]
    string Username,

    [Required(ErrorMessage = "PasswordHash is required")]
    string PasswordHash,

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    string Email,

    [Required(ErrorMessage = "DisplayName is required")]
    string DisplayName,

    string? PhoneNumber,

    DateTime? BirthDate,

    EmployeeType EmployeeType,

    [Required(ErrorMessage = "StaffNumber is required")]
    string StaffNumber,

    [Required(ErrorMessage = "Position is required")]
    string Position,

    string? DepartmentName,
    int? TeamId,
    int? ManagerId,
    string? Certification,
    string? ResponsibilityArea,
    StaffType? StaffType) : IRequest<int>;

public record UpdateEmployeeCommand(
    int EmployeeId,
    string StaffNumber,
    string Position,
    string? DepartmentName,
    int? TeamId,
    int? ManagerId,
    string? Certification,
    string? ResponsibilityArea) : IRequest<Unit>;

public record DeleteEmployeeCommand(int EmployeeId) : IRequest<Unit>;

using DbApp.Domain.Entities.UserSystem;
using MediatR;

namespace DbApp.Application.UserSystem.Employees;

public record CreateEmployeeCommand(
    int EmployeeId,
    string StaffNumber,
    string Position,
    string? DepartmentName,
    int? TeamId,
    int? ManagerId,
    string? Certification,
    string? ResponsibilityArea) : IRequest<int>;

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

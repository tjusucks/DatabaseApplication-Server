using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Enums.UserSystem;
using MediatR;

namespace DbApp.Application.UserSystem.Employees;

public record GetAllEmployeesQuery : IRequest<List<Employee>>;

public record GetEmployeeByIdQuery(int EmployeeId) : IRequest<Employee?>;

public record SearchEmployeesQuery(string Keyword) : IRequest<List<Employee>>;

public record GetEmployeesByDepartmentQuery(string DepartmentName) : IRequest<List<Employee>>;

public record GetEmployeesByStaffTypeQuery(StaffType StaffType) : IRequest<List<Employee>>;

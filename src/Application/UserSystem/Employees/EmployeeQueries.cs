using DbApp.Domain.Enums.UserSystem;
using MediatR;

namespace DbApp.Application.UserSystem.Employees;

public record GetAllEmployeesQuery : IRequest<List<EmployeeDto>>;

public record GetEmployeeByIdQuery(int EmployeeId) : IRequest<EmployeeDto?>;

public record SearchEmployeesQuery(string Keyword) : IRequest<List<EmployeeDto>>;

public record GetEmployeesByDepartmentQuery(string DepartmentName) : IRequest<List<EmployeeDto>>;

public record GetEmployeesByStaffTypeQuery(StaffType StaffType) : IRequest<List<EmployeeDto>>;

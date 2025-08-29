using DbApp.Domain.Entities.UserSystem;
using MediatR;

namespace DbApp.Application.UserSystem.Employees;

public record GetAllEmployeesQuery : IRequest<List<Employee>>;

public record GetEmployeeByIdQuery(int EmployeeId) : IRequest<Employee?>;

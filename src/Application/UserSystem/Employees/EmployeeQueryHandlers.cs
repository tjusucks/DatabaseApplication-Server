using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Interfaces.UserSystem;
using MediatR;

namespace DbApp.Application.UserSystem.Employees;

public class GetAllEmployeesQueryHandler(IEmployeeRepository employeeRepository) : IRequestHandler<GetAllEmployeesQuery, List<Employee>>
{
    private readonly IEmployeeRepository _employeeRepository = employeeRepository;

    public async Task<List<Employee>> Handle(GetAllEmployeesQuery request, CancellationToken cancellationToken)
    {
        return await _employeeRepository.GetAllAsync();
    }
}

public class GetEmployeeByIdQueryHandler(IEmployeeRepository employeeRepository) : IRequestHandler<GetEmployeeByIdQuery, Employee?>
{
    private readonly IEmployeeRepository _employeeRepository = employeeRepository;

    public async Task<Employee?> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
    {
        return await _employeeRepository.GetByIdAsync(request.EmployeeId);
    }
}

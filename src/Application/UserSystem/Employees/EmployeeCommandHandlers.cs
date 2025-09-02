using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Interfaces.UserSystem;
using MediatR;

namespace DbApp.Application.UserSystem.Employees;

public class CreateEmployeeCommandHandler(IEmployeeRepository employeeRepository) : IRequestHandler<CreateEmployeeCommand, int>
{
    private readonly IEmployeeRepository _employeeRepository = employeeRepository;

    public async Task<int> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = new Employee
        {
            EmployeeId = request.EmployeeId,
            StaffNumber = request.StaffNumber,
            Position = request.Position,
            DepartmentName = request.DepartmentName,
            TeamId = request.TeamId,
            ManagerId = request.ManagerId,
            Certification = request.Certification,
            ResponsibilityArea = request.ResponsibilityArea,
            CreatedAt = DateTime.UtcNow
        };

        return await _employeeRepository.CreateAsync(employee);
    }
}

public class UpdateEmployeeCommandHandler(IEmployeeRepository employeeRepository) : IRequestHandler<UpdateEmployeeCommand, Unit>
{
    private readonly IEmployeeRepository _employeeRepository = employeeRepository;

    public async Task<Unit> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId)
            ?? throw new InvalidOperationException("Employee not found");

        employee.StaffNumber = request.StaffNumber;
        employee.Position = request.Position;
        employee.DepartmentName = request.DepartmentName;
        employee.TeamId = request.TeamId;
        employee.ManagerId = request.ManagerId;
        employee.Certification = request.Certification;
        employee.ResponsibilityArea = request.ResponsibilityArea;
        employee.UpdatedAt = DateTime.UtcNow;

        await _employeeRepository.UpdateAsync(employee);
        return Unit.Value;
    }
}

public class DeleteEmployeeCommandHandler(IEmployeeRepository employeeRepository) : IRequestHandler<DeleteEmployeeCommand, Unit>
{
    private readonly IEmployeeRepository _employeeRepository = employeeRepository;

    public async Task<Unit> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId)
            ?? throw new InvalidOperationException("Employee not found");
        await _employeeRepository.DeleteAsync(employee);
        return Unit.Value;
    }
}

using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Interfaces.UserSystem;
using MediatR;
using static DbApp.Domain.Exceptions;

namespace DbApp.Application.UserSystem.Employees;

public class CreateEmployeeCommandHandler(IEmployeeRepository employeeRepository) : IRequestHandler<CreateEmployeeCommand, int>
{
    private readonly IEmployeeRepository _employeeRepository = employeeRepository;

    public async Task<int> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        // Map employee type to role name
        string roleName = request.EmployeeType switch
        {
            EmployeeType.Manager => "Manager",
            EmployeeType.Employee => "Employee",
            _ => throw new ValidationException("Invalid employee type specified.")
        };

        // Get the role ID based on role name
        var roleId = await _employeeRepository.GetRoleIdByNameAsync(roleName)
            ?? throw new ValidationException($"Role '{roleName}' not found in the system.");

        // First create the User
        var user = new User
        {
            Username = request.Username,
            PasswordHash = request.PasswordHash,
            Email = request.Email,
            DisplayName = request.DisplayName,
            PhoneNumber = request.PhoneNumber,
            BirthDate = request.BirthDate,
            RoleId = roleId,
            CreatedAt = DateTime.UtcNow,
            RegisterTime = DateTime.UtcNow
        };

        // Save the user to get the UserId
        var userId = await _employeeRepository.CreateUserAsync(user);

        try
        {
            // Then create the Employee with the UserId
            var employee = new Employee
            {
                EmployeeId = userId, // This links the Employee to the User
                StaffNumber = request.StaffNumber,
                Position = request.Position,
                DepartmentName = request.DepartmentName,
                TeamId = request.TeamId,
                ManagerId = request.ManagerId,
                Certification = request.Certification,
                ResponsibilityArea = request.ResponsibilityArea,
                StaffType = request.StaffType,
                CreatedAt = DateTime.UtcNow,
                HireDate = DateTime.UtcNow
            };

            return await _employeeRepository.CreateAsync(employee);
        }
        catch
        {
            // 如果创建Employee失败，需要删除已创建的User
            await _employeeRepository.DeleteUserAsync(userId);
            throw;
        }
    }
}

public class UpdateEmployeeCommandHandler(IEmployeeRepository employeeRepository) : IRequestHandler<UpdateEmployeeCommand, Unit>
{
    private readonly IEmployeeRepository _employeeRepository = employeeRepository;

    public async Task<Unit> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId)
            ?? throw new NotFoundException($"Employee with ID {request.EmployeeId} not found.");

        // Update employee properties
        employee.StaffNumber = request.StaffNumber;
        employee.Position = request.Position;
        employee.DepartmentName = request.DepartmentName;
        employee.TeamId = request.TeamId;
        employee.ManagerId = request.ManagerId;
        employee.Certification = request.Certification;
        employee.ResponsibilityArea = request.ResponsibilityArea;

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
            ?? throw new NotFoundException($"Employee with ID {request.EmployeeId} not found.");
        await _employeeRepository.DeleteAsync(employee);
        return Unit.Value;
    }
}

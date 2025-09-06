using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Interfaces.UserSystem;
using MediatR;

namespace DbApp.Application.UserSystem.Employees;

public class GetAllEmployeesQueryHandler(IEmployeeRepository employeeRepository) : IRequestHandler<GetAllEmployeesQuery, List<EmployeeDto>>
{
    private readonly IEmployeeRepository _employeeRepository = employeeRepository;

    public async Task<List<EmployeeDto>> Handle(GetAllEmployeesQuery request, CancellationToken cancellationToken)
    {
        var employees = await _employeeRepository.GetAllAsync();
        return employees.Select(MapToDto).ToList();
    }

    private EmployeeDto MapToDto(Employee employee)
    {
        return new EmployeeDto
        {
            EmployeeId = employee.EmployeeId,
            StaffNumber = employee.StaffNumber,
            Position = employee.Position,
            DepartmentName = employee.DepartmentName,
            StaffType = employee.StaffType,
            TeamId = employee.TeamId,
            HireDate = employee.HireDate,
            EmploymentStatus = employee.EmploymentStatus,
            ManagerId = employee.ManagerId,
            Certification = employee.Certification,
            ResponsibilityArea = employee.ResponsibilityArea,
            CreatedAt = employee.CreatedAt,
            UpdatedAt = employee.UpdatedAt,
            Manager = employee.Manager != null ? MapToSimpleDto(employee.Manager) : null,
            Team = employee.Team != null ? new TeamSimpleDto
            {
                TeamId = employee.Team.TeamId,
                TeamName = employee.Team.TeamName,
                TeamType = employee.Team.TeamType.ToString()
            } : null,
            User = new UserSimpleDto
            {
                UserId = employee.User.UserId,
                Username = employee.User.Username,
                Email = employee.User.Email,
                PhoneNumber = employee.User.PhoneNumber ?? string.Empty,
                DisplayName = employee.User.DisplayName
            }
        };
    }

    private EmployeeSimpleDto MapToSimpleDto(Employee employee)
    {
        return new EmployeeSimpleDto
        {
            EmployeeId = employee.EmployeeId,
            StaffNumber = employee.StaffNumber,
            Position = employee.Position,
            DepartmentName = employee.DepartmentName,
            StaffType = employee.StaffType
        };
    }
}

public class GetEmployeeByIdQueryHandler(IEmployeeRepository employeeRepository) : IRequestHandler<GetEmployeeByIdQuery, EmployeeDto?>
{
    private readonly IEmployeeRepository _employeeRepository = employeeRepository;

    public async Task<EmployeeDto?> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId);
        return employee != null ? MapToDto(employee) : null;
    }

    private EmployeeDto MapToDto(Employee employee)
    {
        return new EmployeeDto
        {
            EmployeeId = employee.EmployeeId,
            StaffNumber = employee.StaffNumber,
            Position = employee.Position,
            DepartmentName = employee.DepartmentName,
            StaffType = employee.StaffType,
            TeamId = employee.TeamId,
            HireDate = employee.HireDate,
            EmploymentStatus = employee.EmploymentStatus,
            ManagerId = employee.ManagerId,
            Certification = employee.Certification,
            ResponsibilityArea = employee.ResponsibilityArea,
            CreatedAt = employee.CreatedAt,
            UpdatedAt = employee.UpdatedAt,
            Manager = employee.Manager != null ? MapToSimpleDto(employee.Manager) : null,
            Team = employee.Team != null ? new TeamSimpleDto
            {
                TeamId = employee.Team.TeamId,
                TeamName = employee.Team.TeamName,
                TeamType = employee.Team.TeamType.ToString()
            } : null,
            User = new UserSimpleDto
            {
                UserId = employee.User.UserId,
                Username = employee.User.Username,
                Email = employee.User.Email,
                PhoneNumber = employee.User.PhoneNumber ?? string.Empty,
                DisplayName = employee.User.DisplayName
            }
        };
    }

    private EmployeeSimpleDto MapToSimpleDto(Employee employee)
    {
        return new EmployeeSimpleDto
        {
            EmployeeId = employee.EmployeeId,
            StaffNumber = employee.StaffNumber,
            Position = employee.Position,
            DepartmentName = employee.DepartmentName,
            StaffType = employee.StaffType
        };
    }
}


public class GetEmployeesByDepartmentQueryHandler(IEmployeeRepository employeeRepository) : IRequestHandler<GetEmployeesByDepartmentQuery, List<EmployeeDto>>
{
    private readonly IEmployeeRepository _employeeRepository = employeeRepository;

    public async Task<List<EmployeeDto>> Handle(GetEmployeesByDepartmentQuery request, CancellationToken cancellationToken)
    {
        var employees = await _employeeRepository.GetByDepartmentAsync(request.DepartmentName);
        return employees.Select(MapToDto).ToList();
    }

    private EmployeeDto MapToDto(Employee employee)
    {
        return new EmployeeDto
        {
            EmployeeId = employee.EmployeeId,
            StaffNumber = employee.StaffNumber,
            Position = employee.Position,
            DepartmentName = employee.DepartmentName,
            StaffType = employee.StaffType,
            TeamId = employee.TeamId,
            HireDate = employee.HireDate,
            EmploymentStatus = employee.EmploymentStatus,
            ManagerId = employee.ManagerId,
            Certification = employee.Certification,
            ResponsibilityArea = employee.ResponsibilityArea,
            CreatedAt = employee.CreatedAt,
            UpdatedAt = employee.UpdatedAt,
            Manager = employee.Manager != null ? MapToSimpleDto(employee.Manager) : null,
            Team = employee.Team != null ? new TeamSimpleDto
            {
                TeamId = employee.Team.TeamId,
                TeamName = employee.Team.TeamName,
                TeamType = employee.Team.TeamType.ToString()
            } : null,
            User = new UserSimpleDto
            {
                UserId = employee.User.UserId,
                Username = employee.User.Username,
                Email = employee.User.Email,
                PhoneNumber = employee.User.PhoneNumber ?? string.Empty,
                DisplayName = employee.User.DisplayName
            }
        };
    }

    private EmployeeSimpleDto MapToSimpleDto(Employee employee)
    {
        return new EmployeeSimpleDto
        {
            EmployeeId = employee.EmployeeId,
            StaffNumber = employee.StaffNumber,
            Position = employee.Position,
            DepartmentName = employee.DepartmentName,
            StaffType = employee.StaffType
        };
    }
}

public class SearchEmployeesQueryHandler(IEmployeeRepository employeeRepository) : IRequestHandler<SearchEmployeesQuery, List<EmployeeDto>>
{
    private readonly IEmployeeRepository _employeeRepository = employeeRepository;

    public async Task<List<EmployeeDto>> Handle(SearchEmployeesQuery request, CancellationToken cancellationToken)
    {
        var employees = await _employeeRepository.SearchAsync(request.Keyword);
        return employees.Select(MapToDto).ToList();
    }

    private EmployeeDto MapToDto(Employee employee)
    {
        return new EmployeeDto
        {
            EmployeeId = employee.EmployeeId,
            StaffNumber = employee.StaffNumber,
            Position = employee.Position,
            DepartmentName = employee.DepartmentName,
            StaffType = employee.StaffType,
            TeamId = employee.TeamId,
            HireDate = employee.HireDate,
            EmploymentStatus = employee.EmploymentStatus,
            ManagerId = employee.ManagerId,
            Certification = employee.Certification,
            ResponsibilityArea = employee.ResponsibilityArea,
            CreatedAt = employee.CreatedAt,
            UpdatedAt = employee.UpdatedAt,
            Manager = employee.Manager != null ? MapToSimpleDto(employee.Manager) : null,
            Team = employee.Team != null ? new TeamSimpleDto
            {
                TeamId = employee.Team.TeamId,
                TeamName = employee.Team.TeamName,
                TeamType = employee.Team.TeamType.ToString()
            } : null,
            User = new UserSimpleDto
            {
                UserId = employee.User.UserId,
                Username = employee.User.Username,
                Email = employee.User.Email,
                PhoneNumber = employee.User.PhoneNumber ?? string.Empty,
                DisplayName = employee.User.DisplayName
            }
        };
    }

    private EmployeeSimpleDto MapToSimpleDto(Employee employee)
    {
        return new EmployeeSimpleDto
        {
            EmployeeId = employee.EmployeeId,
            StaffNumber = employee.StaffNumber,
            Position = employee.Position,
            DepartmentName = employee.DepartmentName,
            StaffType = employee.StaffType
        };
    }
}

public class GetEmployeesByStaffTypeQueryHandler(IEmployeeRepository employeeRepository) : IRequestHandler<GetEmployeesByStaffTypeQuery, List<EmployeeDto>>
{
    private readonly IEmployeeRepository _employeeRepository = employeeRepository;

    public async Task<List<EmployeeDto>> Handle(GetEmployeesByStaffTypeQuery request, CancellationToken cancellationToken)
    {
        var employees = await _employeeRepository.GetByStaffTypeAsync(request.StaffType);
        return employees.Select(MapToDto).ToList();
    }

    private EmployeeDto MapToDto(Employee employee)
    {
        return new EmployeeDto
        {
            EmployeeId = employee.EmployeeId,
            StaffNumber = employee.StaffNumber,
            Position = employee.Position,
            DepartmentName = employee.DepartmentName,
            StaffType = employee.StaffType,
            TeamId = employee.TeamId,
            HireDate = employee.HireDate,
            EmploymentStatus = employee.EmploymentStatus,
            ManagerId = employee.ManagerId,
            Certification = employee.Certification,
            ResponsibilityArea = employee.ResponsibilityArea,
            CreatedAt = employee.CreatedAt,
            UpdatedAt = employee.UpdatedAt,
            Manager = employee.Manager != null ? MapToSimpleDto(employee.Manager) : null,
            Team = employee.Team != null ? new TeamSimpleDto
            {
                TeamId = employee.Team.TeamId,
                TeamName = employee.Team.TeamName,
                TeamType = employee.Team.TeamType.ToString()
            } : null,
            User = new UserSimpleDto
            {
                UserId = employee.User.UserId,
                Username = employee.User.Username,
                Email = employee.User.Email,
                PhoneNumber = employee.User.PhoneNumber ?? string.Empty,
                DisplayName = employee.User.DisplayName
            }
        };
    }

    private EmployeeSimpleDto MapToSimpleDto(Employee employee)
    {
        return new EmployeeSimpleDto
        {
            EmployeeId = employee.EmployeeId,
            StaffNumber = employee.StaffNumber,
            Position = employee.Position,
            DepartmentName = employee.DepartmentName,
            StaffType = employee.StaffType
        };
    }
}

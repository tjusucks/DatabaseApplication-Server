using DbApp.Application.UserSystem.Employees;
using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Enums.UserSystem;
using DbApp.Domain.Interfaces.UserSystem;
using DbApp.Presentation.Controllers;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Moq;

namespace DbApp.Tests;

public class EmployeeQueryTest
{
    [Fact]
    public async Task Get_Employees_By_Department_Test()
    {
        // Arrange
        var mockEmployeeRepository = new Mock<IEmployeeRepository>();
        var handler = new GetEmployeesByDepartmentQueryHandler(mockEmployeeRepository.Object);
        
        var departmentName = "IT";
        var expectedEmployees = new List<Employee>
        {
            new Employee
            {
                EmployeeId = 1,
                StaffNumber = "EMP001",
                Position = "Developer",
                DepartmentName = "IT",
                StaffType = StaffType.Regular,
                HireDate = DateTime.UtcNow,
                EmploymentStatus = EmploymentStatus.Active
            },
            new Employee
            {
                EmployeeId = 2,
                StaffNumber = "EMP002",
                Position = "Manager",
                DepartmentName = "IT",
                StaffType = StaffType.Manager,
                HireDate = DateTime.UtcNow,
                EmploymentStatus = EmploymentStatus.Active
            }
        };
        
        mockEmployeeRepository.Setup(repo => repo.GetByDepartmentAsync(departmentName))
            .ReturnsAsync(expectedEmployees);
            
        var query = new GetEmployeesByDepartmentQuery(departmentName);
        
        // Act
        var result = await handler.Handle(query, CancellationToken.None);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedEmployees.Count, result.Count);
        Assert.All(result, employee => Assert.Equal(departmentName, employee.DepartmentName));
        mockEmployeeRepository.Verify(repo => repo.GetByDepartmentAsync(departmentName), Times.Once);
    }
    
    [Fact]
    public async Task Get_Employees_By_Department_With_Empty_Result_Test()
    {
        // Arrange
        var mockEmployeeRepository = new Mock<IEmployeeRepository>();
        var handler = new GetEmployeesByDepartmentQueryHandler(mockEmployeeRepository.Object);
        
        var departmentName = "NonExistentDepartment";
        var expectedEmployees = new List<Employee>();
        
        mockEmployeeRepository.Setup(repo => repo.GetByDepartmentAsync(departmentName))
            .ReturnsAsync(expectedEmployees);
            
        var query = new GetEmployeesByDepartmentQuery(departmentName);
        
        // Act
        var result = await handler.Handle(query, CancellationToken.None);
        
        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        mockEmployeeRepository.Verify(repo => repo.GetByDepartmentAsync(departmentName), Times.Once);
    }
    
    [Fact]
    public async Task Get_Employees_By_StaffType_Test()
    {
        // Arrange
        var mockEmployeeRepository = new Mock<IEmployeeRepository>();
        var handler = new GetEmployeesByStaffTypeQueryHandler(mockEmployeeRepository.Object);
        
        var staffType = StaffType.Manager;
        var expectedEmployees = new List<Employee>
        {
            new Employee
            {
                EmployeeId = 1,
                StaffNumber = "EMP001",
                Position = "Manager",
                DepartmentName = "IT",
                StaffType = StaffType.Manager,
                HireDate = DateTime.UtcNow,
                EmploymentStatus = EmploymentStatus.Active
            },
            new Employee
            {
                EmployeeId = 2,
                StaffNumber = "EMP002",
                Position = "Manager",
                DepartmentName = "HR",
                StaffType = StaffType.Manager,
                HireDate = DateTime.UtcNow,
                EmploymentStatus = EmploymentStatus.Active
            }
        };
        
        mockEmployeeRepository.Setup(repo => repo.GetByStaffTypeAsync(staffType))
            .ReturnsAsync(expectedEmployees);
            
        var query = new GetEmployeesByStaffTypeQuery(staffType);
        
        // Act
        var result = await handler.Handle(query, CancellationToken.None);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedEmployees.Count, result.Count);
        Assert.All(result, employee => Assert.Equal(staffType, employee.StaffType));
        mockEmployeeRepository.Verify(repo => repo.GetByStaffTypeAsync(staffType), Times.Once);
    }
    
    [Fact]
    public async Task Get_Employees_By_StaffType_With_Empty_Result_Test()
    {
        // Arrange
        var mockEmployeeRepository = new Mock<IEmployeeRepository>();
        var handler = new GetEmployeesByStaffTypeQueryHandler(mockEmployeeRepository.Object);
        
        var staffType = StaffType.Inspector;
        var expectedEmployees = new List<Employee>();
        
        mockEmployeeRepository.Setup(repo => repo.GetByStaffTypeAsync(staffType))
            .ReturnsAsync(expectedEmployees);
            
        var query = new GetEmployeesByStaffTypeQuery(staffType);
        
        // Act
        var result = await handler.Handle(query, CancellationToken.None);
        
        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        mockEmployeeRepository.Verify(repo => repo.GetByStaffTypeAsync(staffType), Times.Once);
    }
    
    [Fact]
    public async Task Get_All_Employees_Test()
    {
        // Arrange
        var mockEmployeeRepository = new Mock<IEmployeeRepository>();
        var handler = new GetAllEmployeesQueryHandler(mockEmployeeRepository.Object);
        
        var expectedEmployees = new List<Employee>
        {
            new Employee
            {
                EmployeeId = 1,
                StaffNumber = "EMP001",
                Position = "Developer",
                DepartmentName = "IT",
                StaffType = StaffType.Regular,
                HireDate = DateTime.UtcNow,
                EmploymentStatus = EmploymentStatus.Active
            },
            new Employee
            {
                EmployeeId = 2,
                StaffNumber = "EMP002",
                Position = "Manager",
                DepartmentName = "HR",
                StaffType = StaffType.Manager,
                HireDate = DateTime.UtcNow,
                EmploymentStatus = EmploymentStatus.Active
            },
            new Employee
            {
                EmployeeId = 3,
                StaffNumber = "EMP003",
                Position = "Mechanic",
                DepartmentName = "Maintenance",
                StaffType = StaffType.Mechanic,
                HireDate = DateTime.UtcNow,
                EmploymentStatus = EmploymentStatus.OnLeave
            }
        };
        
        mockEmployeeRepository.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(expectedEmployees);
            
        var query = new GetAllEmployeesQuery();
        
        // Act
        var result = await handler.Handle(query, CancellationToken.None);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedEmployees.Count, result.Count);
        mockEmployeeRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
    }
    
    [Fact]
    public async Task Search_Employees_By_Keyword_Test()
    {
        // Arrange
        var mockEmployeeRepository = new Mock<IEmployeeRepository>();
        var handler = new SearchEmployeesQueryHandler(mockEmployeeRepository.Object);
        
        var keyword = "dev";
        var expectedEmployees = new List<Employee>
        {
            new Employee
            {
                EmployeeId = 1,
                StaffNumber = "EMP001",
                Position = "Developer",
                DepartmentName = "IT",
                StaffType = StaffType.Regular,
                HireDate = DateTime.UtcNow,
                EmploymentStatus = EmploymentStatus.Active
            }
        };
        
        mockEmployeeRepository.Setup(repo => repo.SearchAsync(keyword))
            .ReturnsAsync(expectedEmployees);
            
        var query = new SearchEmployeesQuery(keyword);
        
        // Act
        var result = await handler.Handle(query, CancellationToken.None);
        
        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Contains(expectedEmployees[0], result);
        mockEmployeeRepository.Verify(repo => repo.SearchAsync(keyword), Times.Once);
    }
    
    [Fact]
    public async Task Search_Employees_By_Keyword_With_Empty_Result_Test()
    {
        // Arrange
        var mockEmployeeRepository = new Mock<IEmployeeRepository>();
        var handler = new SearchEmployeesQueryHandler(mockEmployeeRepository.Object);
        
        var keyword = "nonexistent";
        var expectedEmployees = new List<Employee>();
        
        mockEmployeeRepository.Setup(repo => repo.SearchAsync(keyword))
            .ReturnsAsync(expectedEmployees);
            
        var query = new SearchEmployeesQuery(keyword);
        
        // Act
        var result = await handler.Handle(query, CancellationToken.None);
        
        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        mockEmployeeRepository.Verify(repo => repo.SearchAsync(keyword), Times.Once);
    }
    
    [Fact]
    public async Task Get_Employee_By_Id_Test()
    {
        // Arrange
        var mockEmployeeRepository = new Mock<IEmployeeRepository>();
        var handler = new GetEmployeeByIdQueryHandler(mockEmployeeRepository.Object);
        
        var employeeId = 1;
        var expectedEmployee = new Employee
        {
            EmployeeId = employeeId,
            StaffNumber = "EMP001",
            Position = "Developer",
            DepartmentName = "IT",
            StaffType = StaffType.Regular,
            HireDate = DateTime.UtcNow,
            EmploymentStatus = EmploymentStatus.Active
        };
        
        mockEmployeeRepository.Setup(repo => repo.GetByIdAsync(employeeId))
            .ReturnsAsync(expectedEmployee);
            
        var query = new GetEmployeeByIdQuery(employeeId);
        
        // Act
        var result = await handler.Handle(query, CancellationToken.None);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedEmployee.EmployeeId, result.EmployeeId);
        Assert.Equal(expectedEmployee.StaffNumber, result.StaffNumber);
        Assert.Equal(expectedEmployee.Position, result.Position);
        mockEmployeeRepository.Verify(repo => repo.GetByIdAsync(employeeId), Times.Once);
    }
    
    [Fact]
    public async Task Employee_Controller_Get_By_Department_Test()
    {
        // Arrange
        var mockMediator = new Mock<IMediator>();
        var controller = new EmployeesController(mockMediator.Object);
        
        var departmentName = "IT";
        var expectedEmployees = new List<Employee>
        {
            new Employee
            {
                EmployeeId = 1,
                StaffNumber = "EMP001",
                Position = "Developer",
                DepartmentName = "IT",
                StaffType = StaffType.Regular,
                HireDate = DateTime.UtcNow,
                EmploymentStatus = EmploymentStatus.Active
            }
        };
        
        mockMediator.Setup(m => m.Send(It.IsAny<GetEmployeesByDepartmentQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedEmployees);
        
        // Act
        var result = await controller.GetByDepartment(departmentName);
        
        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<List<Employee>>(okResult.Value);
        Assert.Single(returnValue);
        Assert.Equal(departmentName, returnValue[0].DepartmentName);
        mockMediator.Verify(m => m.Send(It.IsAny<GetEmployeesByDepartmentQuery>(), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task Employee_Controller_Get_By_StaffType_Test()
    {
        // Arrange
        var mockMediator = new Mock<IMediator>();
        var controller = new EmployeesController(mockMediator.Object);
        
        var staffType = StaffType.Manager;
        var expectedEmployees = new List<Employee>
        {
            new Employee
            {
                EmployeeId = 1,
                StaffNumber = "EMP001",
                Position = "Manager",
                DepartmentName = "IT",
                StaffType = StaffType.Manager,
                HireDate = DateTime.UtcNow,
                EmploymentStatus = EmploymentStatus.Active
            },
            new Employee
            {
                EmployeeId = 2,
                StaffNumber = "EMP002",
                Position = "Manager",
                DepartmentName = "HR",
                StaffType = StaffType.Manager,
                HireDate = DateTime.UtcNow,
                EmploymentStatus = EmploymentStatus.Active
            }
        };
        
        mockMediator.Setup(m => m.Send(It.IsAny<GetEmployeesByStaffTypeQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedEmployees);
        
        // Act
        var result = await controller.GetByStaffType(staffType);
        
        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<List<Employee>>(okResult.Value);
        Assert.Equal(2, returnValue.Count);
        Assert.All(returnValue, employee => Assert.Equal(staffType, employee.StaffType));
        mockMediator.Verify(m => m.Send(It.IsAny<GetEmployeesByStaffTypeQuery>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
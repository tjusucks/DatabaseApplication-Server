using DbApp.Application.UserSystem.Employees;
using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Enums.UserSystem;
using DbApp.Domain.Interfaces.UserSystem;
using MediatR;
using Moq;

namespace DbApp.Tests;

public class EmployeeTest
{
    [Fact]
    public async Task Employee_CRUD_Operations_With_Output_Test()
    {
        // Arrange
        Console.WriteLine("=== Employee CRUD Operations Test ===");

        var expectedEmployee = new Employee
        {
            EmployeeId = 1,
            StaffNumber = "EMP001",
            Position = "Developer",
            DepartmentName = "IT",
            StaffType = StaffType.Regular,
            TeamId = 1,
            HireDate = DateTime.UtcNow,
            EmploymentStatus = EmploymentStatus.Active,
            ManagerId = null,
            Certification = "Certified Developer",
            ResponsibilityArea = "Backend Development",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };

        var mockEmployeeRepository = new Mock<IEmployeeRepository>();

        // Setup for Create
        mockEmployeeRepository.Setup(repo => repo.CreateAsync(It.IsAny<Employee>()))
            .ReturnsAsync(expectedEmployee.EmployeeId)
            .Callback<Employee>(e =>
            {
                e.EmployeeId = expectedEmployee.EmployeeId;
                Console.WriteLine($"Repository: Created employee with ID {e.EmployeeId}");
            });

        // Setup for GetById
        mockEmployeeRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((int id) =>
            {
                Console.WriteLine($"Repository: Getting employee with ID {id}");
                return expectedEmployee;
            });

        // Setup for GetAll
        mockEmployeeRepository.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(() =>
            {
                Console.WriteLine("Repository: Getting all employees");
                return new List<Employee> { expectedEmployee };
            });

        // Setup for Update
        mockEmployeeRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Employee>()))
            .Returns(() =>
            {
                Console.WriteLine("Repository: Updating employee");
                return Task.CompletedTask;
            });

        // Setup for Delete
        mockEmployeeRepository.Setup(repo => repo.DeleteAsync(It.IsAny<Employee>()))
            .Returns(() =>
            {
                Console.WriteLine("Repository: Deleting employee");
                return Task.CompletedTask;
            });

        Console.WriteLine("\n--- Testing Create Operation ---");
        // Test Create
        var createHandler = new CreateEmployeeCommandHandler(mockEmployeeRepository.Object);
        var createCommand = new CreateEmployeeCommand(
            expectedEmployee.EmployeeId,
            expectedEmployee.StaffNumber,
            expectedEmployee.Position,
            expectedEmployee.DepartmentName,
            expectedEmployee.TeamId,
            expectedEmployee.ManagerId,
            expectedEmployee.Certification,
            expectedEmployee.ResponsibilityArea
        );

        Console.WriteLine("Frontend: Sending CreateEmployeeCommand");
        var createdEmployeeId = await createHandler.Handle(createCommand, default);
        Console.WriteLine($"Frontend: Received created employee ID: {createdEmployeeId}");

        Assert.Equal(expectedEmployee.EmployeeId, createdEmployeeId);
        mockEmployeeRepository.Verify(repo => repo.CreateAsync(It.IsAny<Employee>()), Times.Once);
        Console.WriteLine("✓ Create operation test passed");

        Console.WriteLine("\n--- Testing Get By ID Operation ---");
        // Test Get By Id
        var getByIdHandler = new GetEmployeeByIdQueryHandler(mockEmployeeRepository.Object);
        Console.WriteLine("Frontend: Sending GetEmployeeByIdQuery");
        var employee = await getByIdHandler.Handle(new GetEmployeeByIdQuery(expectedEmployee.EmployeeId), default);

        Assert.NotNull(employee);
        Assert.Equal(expectedEmployee.EmployeeId, employee.EmployeeId);
        Assert.Equal(expectedEmployee.StaffNumber, employee.StaffNumber);
        Assert.Equal(expectedEmployee.Position, employee.Position);
        mockEmployeeRepository.Verify(repo => repo.GetByIdAsync(expectedEmployee.EmployeeId), Times.Once);

        Console.WriteLine($"Frontend: Received employee - ID: {employee.EmployeeId}, " +
            $"Staff Number: {employee.StaffNumber}, Position: {employee.Position}");
        Console.WriteLine("✓ Get by ID operation test passed");

        Console.WriteLine("\n--- Testing Get All Operation ---");
        // Test Get All
        var getAllHandler = new GetAllEmployeesQueryHandler(mockEmployeeRepository.Object);
        Console.WriteLine("Frontend: Sending GetAllEmployeesQuery");
        var employees = await getAllHandler.Handle(new GetAllEmployeesQuery(), default);

        Assert.NotNull(employees);
        Assert.Single(employees);
        Assert.Equal(expectedEmployee.EmployeeId, employees[0].EmployeeId);
        mockEmployeeRepository.Verify(repo => repo.GetAllAsync(), Times.Once);

        Console.WriteLine($"Frontend: Received {employees.Count} employees");
        Console.WriteLine($"Frontend: First employee - ID: {employees[0].EmployeeId}, " +
            $"Staff Number: {employees[0].StaffNumber}, Position: {employees[0].Position}");
        Console.WriteLine("✓ Get all operation test passed");

        Console.WriteLine("\n--- Testing Update Operation ---");
        // Test Update
        var updateHandler = new UpdateEmployeeCommandHandler(mockEmployeeRepository.Object);
        var updateCommand = new UpdateEmployeeCommand(
            expectedEmployee.EmployeeId,
            expectedEmployee.StaffNumber,
            "Senior Developer", // Updated position
            expectedEmployee.DepartmentName,
            expectedEmployee.TeamId,
            expectedEmployee.ManagerId,
            expectedEmployee.Certification,
            expectedEmployee.ResponsibilityArea
        );

        Console.WriteLine("Frontend: Sending UpdateEmployeeCommand with updated position: Senior Developer");
        await updateHandler.Handle(updateCommand, default);
        mockEmployeeRepository.Verify(repo => repo.GetByIdAsync(expectedEmployee.EmployeeId), Times.Exactly(2));
        mockEmployeeRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Employee>()), Times.Once);
        Console.WriteLine("✓ Update operation test passed");

        Console.WriteLine("\n--- Testing Delete Operation ---");
        // Test Delete
        var deleteHandler = new DeleteEmployeeCommandHandler(mockEmployeeRepository.Object);
        var deleteCommand = new DeleteEmployeeCommand(expectedEmployee.EmployeeId);

        Console.WriteLine("Frontend: Sending DeleteEmployeeCommand");
        await deleteHandler.Handle(deleteCommand, default);
        mockEmployeeRepository.Verify(repo => repo.GetByIdAsync(expectedEmployee.EmployeeId), Times.Exactly(3));
        mockEmployeeRepository.Verify(repo => repo.DeleteAsync(It.IsAny<Employee>()), Times.Once);
        Console.WriteLine("✓ Delete operation test passed");

        Console.WriteLine("\n=== All Employee CRUD Operations Tests Passed ===");
    }

    [Fact]
    public async Task Employee_Search_Operations_With_Output_Test()
    {
        // Arrange
        Console.WriteLine("=== Employee Search Operations Test ===");

        var employeesList = new List<Employee>
        {
            new Employee
            {
                EmployeeId = 1,
                StaffNumber = "EMP001",
                Position = "Developer",
                DepartmentName = "IT Department",
                StaffType = StaffType.Regular,
                TeamId = 1,
                HireDate = DateTime.UtcNow,
                EmploymentStatus = EmploymentStatus.Active,
                ManagerId = null,
                Certification = "Certified Developer",
                ResponsibilityArea = "Backend Development",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            },
            new Employee
            {
                EmployeeId = 2,
                StaffNumber = "EMP002",
                Position = "Manager",
                DepartmentName = "Human Resources",
                StaffType = StaffType.Manager,
                TeamId = 2,
                HireDate = DateTime.UtcNow,
                EmploymentStatus = EmploymentStatus.Active,
                ManagerId = null,
                Certification = "Management Expert",
                ResponsibilityArea = "HR Management",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            },
            new Employee
            {
                EmployeeId = 3,
                StaffNumber = "EMP003",
                Position = "Analyst",
                DepartmentName = "IT Support",
                StaffType = StaffType.Regular,
                TeamId = 1,
                HireDate = DateTime.UtcNow,
                EmploymentStatus = EmploymentStatus.Active,
                ManagerId = 1,
                Certification = "Data Analyst",
                ResponsibilityArea = "Data Analysis",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            }
        };

        var mockEmployeeRepository = new Mock<IEmployeeRepository>();

        // Setup for Search
        mockEmployeeRepository.Setup(repo => repo.SearchAsync(It.IsAny<string>()))
            .ReturnsAsync((string keyword) =>
            {
                Console.WriteLine($"Repository: Searching employees with keyword '{keyword}'");
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    return employeesList;
                }
                
                return employeesList.Where(e => 
                    e.DepartmentName != null && e.DepartmentName.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();
            });

        Console.WriteLine("\n--- Testing Search Operation ---");
        // Test Search
        var searchHandler = new SearchEmployeesQueryHandler(mockEmployeeRepository.Object);
        
        // Search with "IT" keyword - should return 2 employees (IT Department and IT Support)
        Console.WriteLine("Frontend: Sending SearchEmployeesQuery with keyword 'IT'");
        var searchResult = await searchHandler.Handle(new SearchEmployeesQuery("IT"), default);
        
        Assert.NotNull(searchResult);
        Assert.Equal(2, searchResult.Count);
        Assert.Contains(searchResult, e => e.EmployeeId == 1); // IT Department
        Assert.Contains(searchResult, e => e.EmployeeId == 3); // IT Support
        mockEmployeeRepository.Verify(repo => repo.SearchAsync("IT"), Times.Once);
        
        Console.WriteLine($"Frontend: Received {searchResult.Count} employees matching keyword 'IT'");
        foreach (var emp in searchResult)
        {
            Console.WriteLine($"  - Employee ID: {emp.EmployeeId}, Department: {emp.DepartmentName}");
        }
        Console.WriteLine("✓ Search operation test passed");

        // Search with "Human" keyword - should return 1 employee
        Console.WriteLine("\nFrontend: Sending SearchEmployeesQuery with keyword 'Human'");
        var searchResult2 = await searchHandler.Handle(new SearchEmployeesQuery("Human"), default);
        
        Assert.NotNull(searchResult2);
        Assert.Single(searchResult2);
        Assert.Equal(2, searchResult2[0].EmployeeId); // Human Resources
        mockEmployeeRepository.Verify(repo => repo.SearchAsync("Human"), Times.Once);
        
        Console.WriteLine($"Frontend: Received {searchResult2.Count} employee matching keyword 'Human'");
        Console.WriteLine($"  - Employee ID: {searchResult2[0].EmployeeId}, Department: {searchResult2[0].DepartmentName}");
        Console.WriteLine("✓ Search operation with 'Human' keyword test passed");

        // Search with empty keyword - should return all employees
        Console.WriteLine("\nFrontend: Sending SearchEmployeesQuery with empty keyword");
        var searchResult3 = await searchHandler.Handle(new SearchEmployeesQuery(""), default);
        
        Assert.NotNull(searchResult3);
        Assert.Equal(3, searchResult3.Count);
        mockEmployeeRepository.Verify(repo => repo.SearchAsync(""), Times.Once);
        
        Console.WriteLine($"Frontend: Received {searchResult3.Count} employees with empty keyword (all employees)");
        Console.WriteLine("✓ Search operation with empty keyword test passed");

        Console.WriteLine("\n=== All Employee Search Operations Tests Passed ===");
    }
}
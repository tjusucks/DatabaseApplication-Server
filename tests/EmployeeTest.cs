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
}

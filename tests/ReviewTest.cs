using DbApp.Application.ResourceSystem.EmployeeReviews;
using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Enums.ResourceSystem;
using DbApp.Domain.Interfaces.ResourceSystem;
using MediatR;
using Moq;

namespace DbApp.Tests;

public class EmployeeReviewTest
{
    [Fact]
    public async Task EmployeeReview_CRUD_Operations_Test()
    {
        // Arrange
        Console.WriteLine("=== EmployeeReview CRUD Operations Test ===");

        var employee = new Employee
        {
            EmployeeId = 1,
            StaffNumber = "EMP001",
            Position = "Developer",
            DepartmentName = "IT"
        };

        var evaluator = new Employee
        {
            EmployeeId = 2,
            StaffNumber = "EMP002",
            Position = "Manager",
            DepartmentName = "IT"
        };

        var expectedReview = new EmployeeReview
        {
            ReviewId = 1,
            EmployeeId = employee.EmployeeId,
            Period = "2025Q1",
            Score = 95.5m,
            EvaluationLevel = EvaluationLevel.Excellent,
            EvaluatorId = evaluator.EmployeeId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Employee = employee,
            Evaluator = evaluator
        };

        // 创建DTO对象用于模拟mediator的返回值
        var expectedReviewDto = new EmployeeReviewDto
        {
            ReviewId = 1,
            EmployeeId = employee.EmployeeId,
            Period = "2025Q1",
            Score = 95.5m,
            EvaluationLevel = "Excellent",
            EvaluatorId = evaluator.EmployeeId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Employee = new EmployeeSimpleDto
            {
                EmployeeId = employee.EmployeeId,
                StaffNumber = "EMP001",
                Position = "Developer",
                DepartmentName = "IT"
            }
        };

        var mockEmployeeReviewRepository = new Mock<IEmployeeReviewRepository>();

        // Setup for Create
        mockEmployeeReviewRepository.Setup(repo => repo.CreateAsync(It.IsAny<EmployeeReview>()))
            .ReturnsAsync(expectedReview.ReviewId)
            .Callback<EmployeeReview>(r =>
            {
                r.ReviewId = expectedReview.ReviewId;
                Console.WriteLine($"Repository: Created employee review with ID {r.ReviewId}");
            });

        // Setup for GetById
        mockEmployeeReviewRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((int id) =>
            {
                Console.WriteLine($"Repository: Getting employee review with ID {id}");
                return expectedReview;
            });

        // Setup for Update
        mockEmployeeReviewRepository.Setup(repo => repo.UpdateAsync(It.IsAny<EmployeeReview>()))
            .Callback<EmployeeReview>(r =>
            {
                Console.WriteLine($"Repository: Updated employee review with ID {r.ReviewId}");
            })
            .Returns(Task.CompletedTask);

        // Setup for Delete
        mockEmployeeReviewRepository.Setup(repo => repo.DeleteAsync(It.IsAny<EmployeeReview>()))
            .Callback<EmployeeReview>(r =>
            {
                Console.WriteLine($"Repository: Deleted employee review with ID {r.ReviewId}");
            })
            .Returns(Task.CompletedTask);

        // Setup for GetByEmployeeAsync
        mockEmployeeReviewRepository.Setup(repo => repo.GetByEmployeeAsync(It.IsAny<int>()))
            .ReturnsAsync((int employeeId) =>
            {
                Console.WriteLine($"Repository: Getting employee reviews for employee ID {employeeId}");
                return new List<EmployeeReview> { expectedReview };
            });

        var mediatorMock = new Mock<IMediator>();

        // Setup mediator for CreateEmployeeReviewCommand
        mediatorMock.Setup(m => m.Send(It.IsAny<CreateEmployeeReviewCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedReview.ReviewId);

        // Setup mediator for GetEmployeeReviewByIdQuery - 修复类型不匹配问题
        mediatorMock.Setup(m => m.Send(It.IsAny<GetEmployeeReviewByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedReviewDto);

        // Setup mediator for GetEmployeeReviewsByEmployeeQuery - 修复类型不匹配问题
        mediatorMock.Setup(m => m.Send(It.IsAny<GetEmployeeReviewsByEmployeeQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<EmployeeReviewDto> { expectedReviewDto });

        Console.WriteLine("1. Testing Create EmployeeReview Command");
        var createCommand = new CreateEmployeeReviewCommand(
            expectedReview.EmployeeId,
            expectedReview.Period,
            expectedReview.Score,
            expectedReview.EvaluationLevel,
            expectedReview.EvaluatorId);

        var createHandler = new CreateEmployeeReviewCommandHandler(mockEmployeeReviewRepository.Object);
        var createdId = await createHandler.Handle(createCommand, CancellationToken.None);
        Assert.Equal(expectedReview.ReviewId, createdId);
        Console.WriteLine($"   Created EmployeeReview with ID: {createdId}");

        Console.WriteLine("2. Testing Get EmployeeReview By ID Query");
        var getQuery = new GetEmployeeReviewByIdQuery(expectedReview.ReviewId);
        var getHandler = new GetEmployeeReviewByIdQueryHandler(mockEmployeeReviewRepository.Object);
        var retrievedReview = await getHandler.Handle(getQuery, CancellationToken.None);
        Assert.NotNull(retrievedReview);
        Assert.Equal(expectedReview.ReviewId, retrievedReview.ReviewId);
        Assert.Equal(expectedReview.Score, retrievedReview.Score);
        Console.WriteLine($"   Retrieved EmployeeReview with ID: {retrievedReview.ReviewId}, Score: {retrievedReview.Score}");

        Console.WriteLine("3. Testing Get EmployeeReviews By Employee Query");
        var getByEmployeeQuery = new GetEmployeeReviewsByEmployeeQuery(expectedReview.EmployeeId);
        var getByEmployeeHandler = new GetEmployeeReviewsByEmployeeQueryHandler(mockEmployeeReviewRepository.Object);
        var reviewsByEmployee = await getByEmployeeHandler.Handle(getByEmployeeQuery, CancellationToken.None);
        Assert.NotNull(reviewsByEmployee);
        Assert.Single(reviewsByEmployee);
        Assert.Equal(expectedReview.ReviewId, reviewsByEmployee[0].ReviewId);
        Console.WriteLine($"   Found {reviewsByEmployee.Count} reviews for employee ID {expectedReview.EmployeeId}");

        Console.WriteLine("4. Testing Update EmployeeReview Command");
        var updateCommand = new UpdateEmployeeReviewCommand(
            expectedReview.ReviewId,
            90.0m, // New score
            EvaluationLevel.Good, // New level
            expectedReview.EvaluatorId);

        var updateHandler = new UpdateEmployeeReviewCommandHandler(mockEmployeeReviewRepository.Object);
        await updateHandler.Handle(updateCommand, CancellationToken.None);
        Console.WriteLine("   Updated EmployeeReview");

        Console.WriteLine("5. Testing Delete EmployeeReview Command");
        var deleteCommand = new DeleteEmployeeReviewCommand(expectedReview.ReviewId);
        var deleteHandler = new DeleteEmployeeReviewCommandHandler(mockEmployeeReviewRepository.Object);
        await deleteHandler.Handle(deleteCommand, CancellationToken.None);
        Console.WriteLine("   Deleted EmployeeReview");

        Console.WriteLine("=== EmployeeReview CRUD Operations Test Completed Successfully ===");
    }
}

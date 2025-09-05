using DbApp.Application.ResourceSystem.EmployeeReviews;
using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Enums.ResourceSystem;
using DbApp.Domain.Interfaces.ResourceSystem;
using Moq;
using Xunit;

namespace DbApp.Tests
{
    public class EmployeeReviewTests
    {
        private readonly Mock<IEmployeeReviewRepository> _mockEmployeeReviewRepository;
        private readonly Mock<IAttendanceRepository> _mockAttendanceRepository;
        private readonly CreateEmployeeReviewCommandHandler _createEmployeeReviewHandler;
        private readonly UpdateEmployeeReviewCommandHandler _updateEmployeeReviewHandler;
        private readonly DeleteEmployeeReviewCommandHandler _deleteEmployeeReviewHandler;
        private readonly CreateAttendanceDeductionCommandHandler _createAttendanceDeductionHandler;

        public EmployeeReviewTests()
        {
            _mockEmployeeReviewRepository = new Mock<IEmployeeReviewRepository>();
            _mockAttendanceRepository = new Mock<IAttendanceRepository>();

            _createEmployeeReviewHandler = new CreateEmployeeReviewCommandHandler(_mockEmployeeReviewRepository.Object);
            _updateEmployeeReviewHandler = new UpdateEmployeeReviewCommandHandler(_mockEmployeeReviewRepository.Object);
            _deleteEmployeeReviewHandler = new DeleteEmployeeReviewCommandHandler(_mockEmployeeReviewRepository.Object);
            _createAttendanceDeductionHandler = new CreateAttendanceDeductionCommandHandler(
                _mockEmployeeReviewRepository.Object,
                _mockAttendanceRepository.Object);
        }

        [Fact]
        public async Task CreateEmployeeReviewCommandHandler_Should_Create_New_Review()
        {
            // Arrange
            var command = new CreateEmployeeReviewCommand(
                EmployeeId: 1,
                Period: "2025-08",
                Score: 95.5m,
                EvaluationLevel: EvaluationLevel.Excellent,
                EvaluatorId: 2
            );

            _mockEmployeeReviewRepository
                .Setup(x => x.CreateAsync(It.IsAny<EmployeeReview>()))
                .ReturnsAsync(1);

            // Act
            var result = await _createEmployeeReviewHandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(1, result);
            _mockEmployeeReviewRepository.Verify(x => x.CreateAsync(It.Is<EmployeeReview>(r =>
                r.EmployeeId == command.EmployeeId &&
                r.Period == command.Period &&
                r.Score == command.Score &&
                r.EvaluationLevel == command.EvaluationLevel &&
                r.EvaluatorId == command.EvaluatorId)), Times.Once);
        }

        [Fact]
        public async Task UpdateEmployeeReviewCommandHandler_Should_Update_Existing_Review()
        {
            // Arrange
            var existingReview = new EmployeeReview
            {
                ReviewId = 1,
                EmployeeId = 1,
                Period = "2025-08",
                Score = 80.0m,
                EvaluationLevel = EvaluationLevel.Good,
                EvaluatorId = 2,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            };

            var command = new UpdateEmployeeReviewCommand(
                ReviewId: 1,
                Score: 90.0m,
                EvaluationLevel: EvaluationLevel.Excellent,
                EvaluatorId: 3
            );

            _mockEmployeeReviewRepository
                .Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(existingReview);

            _mockEmployeeReviewRepository
                .Setup(x => x.UpdateAsync(It.IsAny<EmployeeReview>()))
                .Returns(Task.CompletedTask);

            // Act
            await _updateEmployeeReviewHandler.Handle(command, CancellationToken.None);

            // Assert
            _mockEmployeeReviewRepository.Verify(x => x.GetByIdAsync(1), Times.Once);
            _mockEmployeeReviewRepository.Verify(x => x.UpdateAsync(It.Is<EmployeeReview>(r =>
                r.ReviewId == 1 &&
                r.Score == 90.0m &&
                r.EvaluationLevel == EvaluationLevel.Excellent &&
                r.EvaluatorId == 3)), Times.Once);
        }

        [Fact]
        public async Task UpdateEmployeeReviewCommandHandler_Should_Throw_Exception_When_Review_Not_Found()
        {
            // Arrange
            var command = new UpdateEmployeeReviewCommand(
                ReviewId: 999,
                Score: 90.0m,
                EvaluationLevel: EvaluationLevel.Excellent,
                EvaluatorId: 3
            );

            EmployeeReview? nullReview = null;
            _mockEmployeeReviewRepository
                .Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(nullReview);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _updateEmployeeReviewHandler.Handle(command, CancellationToken.None));

            Assert.Contains("未找到ID为 999 的员工绩效记录", exception.Message);
        }

        [Fact]
        public async Task DeleteEmployeeReviewCommandHandler_Should_Delete_Existing_Review()
        {
            // Arrange
            var existingReview = new EmployeeReview
            {
                ReviewId = 1,
                EmployeeId = 1,
                Period = "2025-08",
                Score = 80.0m,
                EvaluationLevel = EvaluationLevel.Good,
                EvaluatorId = 2
            };

            var command = new DeleteEmployeeReviewCommand(ReviewId: 1);

            _mockEmployeeReviewRepository
                .Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(existingReview);

            _mockEmployeeReviewRepository
                .Setup(x => x.DeleteAsync(It.IsAny<EmployeeReview>()))
                .Returns(Task.CompletedTask);

            // Act
            await _deleteEmployeeReviewHandler.Handle(command, CancellationToken.None);

            // Assert
            _mockEmployeeReviewRepository.Verify(x => x.GetByIdAsync(1), Times.Once);
            _mockEmployeeReviewRepository.Verify(x => x.DeleteAsync(existingReview), Times.Once);
        }

        [Fact]
        public async Task DeleteEmployeeReviewCommandHandler_Should_Throw_Exception_When_Review_Not_Found()
        {
            // Arrange
            var command = new DeleteEmployeeReviewCommand(ReviewId: 999);

            EmployeeReview? nullReview = null;
            _mockEmployeeReviewRepository
                .Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(nullReview);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _deleteEmployeeReviewHandler.Handle(command, CancellationToken.None));

            Assert.Contains("未找到ID为 999 的员工绩效记录", exception.Message);
        }

        [Fact]
        public async Task CreateAttendanceDeductionCommandHandler_Should_Create_Deduction_Review_For_Late_Employee()
        {
            // Arrange
            var command = new CreateAttendanceDeductionCommand(
                EmployeeId: 1,
                Period: "2025-08",
                EvaluatorId: 2
            );

            // 员工在这个周期内迟到2天，缺勤1天
            _mockAttendanceRepository
                .Setup(x => x.GetEmployeeStatsAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync((presentDays: 19, lateDays: 2, absentDays: 1, leaveDays: 2));

            _mockEmployeeReviewRepository
                .Setup(x => x.CreateAsync(It.IsAny<EmployeeReview>()))
                .ReturnsAsync(1);

            // Act
            var result = await _createAttendanceDeductionHandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(1, result);
            _mockEmployeeReviewRepository.Verify(x => x.CreateAsync(It.Is<EmployeeReview>(r =>
                r.EmployeeId == command.EmployeeId &&
                r.Period == command.Period &&
                r.Score == -5.0m && // 2次迟到*1 + 1次缺勤*3 = 5分扣分，用负数表示
                r.EvaluatorId == command.EvaluatorId)), Times.Once);
        }

        [Fact]
        public async Task CreateAttendanceDeductionCommandHandler_Should_Create_Deduction_Review_For_Absent_Employee()
        {
            // Arrange
            var command = new CreateAttendanceDeductionCommand(
                EmployeeId: 1,
                Period: "2025-08",
                EvaluatorId: 2
            );

            // 员工在这个周期内迟到0天，缺勤3天
            _mockAttendanceRepository
                .Setup(x => x.GetEmployeeStatsAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync((presentDays: 18, lateDays: 0, absentDays: 3, leaveDays: 3));

            _mockEmployeeReviewRepository
                .Setup(x => x.CreateAsync(It.IsAny<EmployeeReview>()))
                .ReturnsAsync(1);

            // Act
            var result = await _createAttendanceDeductionHandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(1, result);
            _mockEmployeeReviewRepository.Verify(x => x.CreateAsync(It.Is<EmployeeReview>(r =>
                r.EmployeeId == command.EmployeeId &&
                r.Period == command.Period &&
                r.Score == -9.0m && // 0次迟到*1 + 3次缺勤*3 = 9分扣分，用负数表示
                r.EvaluatorId == command.EvaluatorId)), Times.Once);
        }

        [Fact]
        public async Task CreateAttendanceDeductionCommandHandler_Should_Limit_Maximum_Deduction_To_20()
        {
            // Arrange
            var command = new CreateAttendanceDeductionCommand(
                EmployeeId: 1,
                Period: "2025-08",
                EvaluatorId: 2
            );

            // 员工在这个周期内迟到10天，缺勤10天，扣分应该为40分，但最大限制为20分
            _mockAttendanceRepository
                .Setup(x => x.GetEmployeeStatsAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync((presentDays: 11, lateDays: 10, absentDays: 10, leaveDays: 0));

            _mockEmployeeReviewRepository
                .Setup(x => x.CreateAsync(It.IsAny<EmployeeReview>()))
                .ReturnsAsync(1);

            // Act
            var result = await _createAttendanceDeductionHandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(1, result);
            _mockEmployeeReviewRepository.Verify(x => x.CreateAsync(It.Is<EmployeeReview>(r =>
                r.EmployeeId == command.EmployeeId &&
                r.Period == command.Period &&
                r.Score == -20.0m && // 最大扣分限制为20分，用负数表示
                r.EvaluatorId == command.EvaluatorId)), Times.Once);
        }

        [Fact]
        public void CalculateDeductionScore_Should_Calculate_Correct_Score()
        {
            // Act
            // 使用反射调用私有方法
            var method = typeof(CreateAttendanceDeductionCommandHandler)
                .GetMethod("CalculateDeductionScore", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

            var result1Obj = method?.Invoke(null, new object[] { 2, 1 }); // 2次迟到 + 1次缺勤
            Assert.NotNull(result1Obj);
            var result1 = (decimal)result1Obj!;

            var result2Obj = method?.Invoke(null, new object[] { 0, 0 }); // 全勤
            Assert.NotNull(result2Obj);
            var result2 = (decimal)result2Obj!;

            var result3Obj = method?.Invoke(null, new object[] { 10, 10 }); // 超过最大限制
            Assert.NotNull(result3Obj);
            var result3 = (decimal)result3Obj!;

            // 添加断言来使用这些变量
            Assert.Equal(-5.0m, result1); // 2次迟到*1 + 1次缺勤*3 = 5分，用负数表示
            Assert.Equal(0.0m, result2); // 全勤得0分
            Assert.Equal(-20.0m, result3); // 超过最大限制时应为20分，用负数表示
        }
    }
}

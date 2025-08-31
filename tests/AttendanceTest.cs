using DbApp.Application.ResourceSystem.Attendances;
using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Enums.ResourceSystem;
using DbApp.Domain.Interfaces.ResourceSystem;
using Moq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DbApp.Tests
{
    public class AttendanceTests
    {
        private readonly Mock<IAttendanceRepository> _mockRepo = new();

        [Fact]
        public async Task CreateAttendance_ShouldAddNewRecord()
        {
            // Arrange
            var handler = new CreateAttendanceCommandHandler(_mockRepo.Object);
            var command = new CreateAttendanceCommand(
                EmployeeId: 1,
                AttendanceDate: DateTime.Today,
                CheckInTime: DateTime.Now,
                CheckOutTime: null,
                Status: AttendanceStatus.Present,
                LeaveType: null);

            int expectedId = 100;

            _mockRepo.Setup(r => r.GetByEmployeeAndDateAsync(1, It.IsAny<DateTime>()))
                     .ReturnsAsync((Attendance?)null); // 修复1
            
            _mockRepo.Setup(r => r.AddAsync(It.IsAny<Attendance>()))
                     .Callback<Attendance>(a => a.AttendanceId = expectedId)
                     .Returns(Task.CompletedTask);

            // Act
            var id = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(expectedId, id);
            _mockRepo.Verify(r => r.AddAsync(It.IsAny<Attendance>()), Times.Once);
        }

        [Fact]
        public async Task CreateAttendance_DuplicateDate_ShouldThrowException()
        {
            // Arrange
            var handler = new CreateAttendanceCommandHandler(_mockRepo.Object);
            var command = new CreateAttendanceCommand(1, DateTime.Today, null, null, AttendanceStatus.Present, null);

            _mockRepo.Setup(r => r.GetByEmployeeAndDateAsync(1, It.IsAny<DateTime>()))
                     .ReturnsAsync(new Attendance()); // 修复2

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task RecordCheckIn_OnTime_ShouldSetPresentStatus()
        {
            // Arrange
            var handler = new RecordCheckInCommandHandler(_mockRepo.Object);
            var command = new RecordCheckInCommand(
                1, 
                DateTime.Parse( // 修复3
                    "2023-01-01 08:59:00", 
                    CultureInfo.InvariantCulture
                )
            );

            _mockRepo.Setup(r => r.GetByEmployeeAndDateAsync(1, It.IsAny<DateTime>()))
                     .ReturnsAsync((Attendance?)null); // 修复1

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            _mockRepo.Verify(r => r.AddAsync(It.Is<Attendance>(a => 
                a.AttendanceStatus == AttendanceStatus.Present)), Times.Once);
        }

        [Fact]
        public async Task RecordCheckIn_Late_ShouldSetLateStatus()
        {
            // Arrange
            var handler = new RecordCheckInCommandHandler(_mockRepo.Object);
            var command = new RecordCheckInCommand(
                1, 
                DateTime.Parse( // 修复3
                    "2023-01-01 09:01:00", 
                    CultureInfo.InvariantCulture
                )
            );

            _mockRepo.Setup(r => r.GetByEmployeeAndDateAsync(1, It.IsAny<DateTime>()))
                     .ReturnsAsync((Attendance?)null); // 修复1

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            _mockRepo.Verify(r => r.AddAsync(It.Is<Attendance>(a => 
                a.AttendanceStatus == AttendanceStatus.Late)), Times.Once);
        }

        [Fact]
        public async Task ApplyLeave_NewRecord_ShouldCreateLeaveAttendance()
        {
            // Arrange
            var handler = new ApplyLeaveCommandHandler(_mockRepo.Object);
            var command = new ApplyLeaveCommand(1, DateTime.Today, LeaveType.Annual);

            _mockRepo.Setup(r => r.GetByEmployeeAndDateAsync(1, It.IsAny<DateTime>()))
                     .ReturnsAsync((Attendance?)null); // 修复1

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            _mockRepo.Verify(r => r.AddAsync(It.Is<Attendance>(a => 
                a.AttendanceStatus == AttendanceStatus.Leave && 
                a.LeaveType == LeaveType.Annual)), Times.Once);
        }

        [Fact]
        public async Task GetEmployeeStats_ShouldCalculateCorrectValues()
        {
            // Arrange
            var handler = new GetEmployeeStatsQueryHandler(_mockRepo.Object);
            var query = new GetEmployeeStatsQuery(1, null, null);
            
            _mockRepo.Setup(r => r.GetEmployeeStatsAsync(1, null, null))
                     .ReturnsAsync((3, 2, 1, 4));

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(3, result.PresentDays);
            Assert.Equal(2, result.LateDays);
            Assert.Equal(1, result.AbsentDays);
            Assert.Equal(4, result.LeaveDays);
            Assert.Equal(10, result.TotalWorkingDays);
            Assert.Equal(70, result.AttendanceRate);
        }

        [Fact]
        public async Task GetMonthlyStats_ShouldCalculateCorrectValues()
        {
            // Arrange
            var handler = new GetEmployeeMonthlyStatsQueryHandler(_mockRepo.Object);
            var query = new GetEmployeeMonthlyStatsQuery(1, 2023, 1);
            
            _mockRepo.Setup(r => r.GetEmployeeStatsAsync(1, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                     .ReturnsAsync((20, 0, 0, 5));

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(20, result.PresentDays);
            Assert.Equal(0, result.LateDays);
            Assert.Equal(0, result.AbsentDays);
            Assert.Equal(5, result.LeaveDays);
            Assert.True(result.IsFullAttendance);
        }

        [Fact]
        public async Task GetAbnormalRecords_ShouldReturnFilteredResults()
        {
            // Arrange
            var handler = new GetAbnormalRecordsQueryHandler(_mockRepo.Object);
            var query = new GetAbnormalRecordsQuery(null, DateTime.Today.AddDays(-7), DateTime.Today);
            
            var abnormalRecords = new List<Attendance>
            {
                new() { AttendanceStatus = AttendanceStatus.Late },
                new() { AttendanceStatus = AttendanceStatus.Absent }
            };
            
            _mockRepo.Setup(r => r.GetAbnormalRecordsAsync(null, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                     .ReturnsAsync(abnormalRecords);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.DoesNotContain(result, r => r.AttendanceStatus == AttendanceStatus.Present);
        }

        [Fact]
        public async Task GetStats_NoRecords_ShouldReturnZeroValues()
        {
            // Arrange
            var handler = new GetEmployeeStatsQueryHandler(_mockRepo.Object);
            var query = new GetEmployeeStatsQuery(1, null, null);
            
            _mockRepo.Setup(r => r.GetEmployeeStatsAsync(1, null, null))
                     .ReturnsAsync((0, 0, 0, 0));

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(0, result.TotalWorkingDays);
            Assert.Equal(0, result.AttendanceRate);
        }

        [Fact]
        public async Task RecordCheckOut_NoCheckIn_ShouldCreateAbsentRecord()
        {
            // Arrange
            var handler = new RecordCheckOutCommandHandler(_mockRepo.Object);
            var command = new RecordCheckOutCommand(1, DateTime.Now);
            
            _mockRepo.Setup(r => r.GetByEmployeeAndDateAsync(1, It.IsAny<DateTime>()))
                     .ReturnsAsync((Attendance?)null); // 修复1

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            _mockRepo.Verify(r => r.AddAsync(It.Is<Attendance>(a => 
                a.AttendanceStatus == AttendanceStatus.Absent)), Times.Once);
        }

        [Fact]
        public async Task UpdateAttendanceStatus_ShouldUpdateCorrectRecord()
        {
            // Arrange
            var handler = new UpdateAttendanceCommandHandler(_mockRepo.Object);
            var attendance = new Attendance { AttendanceId = 1 };
            var command = new UpdateAttendanceCommand(1, null, null, AttendanceStatus.Leave, LeaveType.Sick);
            
            _mockRepo.Setup(r => r.GetByIdAsync(1))
                     .ReturnsAsync(attendance);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(AttendanceStatus.Leave, attendance.AttendanceStatus);
            Assert.Equal(LeaveType.Sick, attendance.LeaveType);
            _mockRepo.Verify(r => r.UpdateAsync(attendance), Times.Once);
        }
    }
}
using System.Net;
using System.Net.Http.Json;
using DbApp.Tests.Fixtures;
using DbApp.Application.TicketingSystem.Tickets;
using DbApp.Application.TicketingSystem.Reservations;
using DbApp.Domain.Entities.TicketingSystem;
using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Enums.TicketingSystem;
using DbApp.Domain.Enums.UserSystem;
using Microsoft.EntityFrameworkCore;

namespace DbApp.Tests.Integrations.TicketingSystem.Tickets;

/// <summary>
/// 退票功能综合集成测试
/// 测试完整的退票流程：创建预订 -> 支付 -> 生成票据 -> 退票申请 -> 退票处理
/// 注重事务原子性和CRUD操作的正确性
/// </summary>
[Collection("Database")]
public class RefundIntegrationTests(DatabaseFixture fixture) : IAsyncLifetime
{
    public Task InitializeAsync() => Task.CompletedTask;
    public async Task DisposeAsync() => await fixture.ResetDatabaseAsync();

    [Fact]
    public async Task RequestRefund_WithValidTicket_ShouldSucceed()
    {
        // Arrange
        var (ticketId, visitorId) = await SeedBasicTestDataAsync();
        
        using var factory = new TestApiFactory(fixture);
        using var client = factory.CreateClient();

        var request = new RequestRefundCommand
        {
            TicketId = ticketId,
            RequestingVisitorId = visitorId,
            RefundReason = "Change of plans",
            IsAdminRequest = false
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/ticketing/refunds/request", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<RefundResultDto>();
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.True(result.RefundAmount > 0);
        Assert.Equal(RefundStatus.Pending, result.Status); // 普通用户申请应该是待审核状态
    }

    [Fact]
    public async Task RefundUnauthorizedTicket_ShouldFail()
    {
        // Arrange
        var (ticketId, _) = await SeedBasicTestDataAsync();
        
        using var factory = new TestApiFactory(fixture);
        using var client = factory.CreateClient();

        // 尝试退别人的票
        var refundRequest = new RequestRefundCommand
        {
            TicketId = ticketId,
            RequestingVisitorId = 9999, // 不存在的访客ID
            RefundReason = "尝试退别人的票",
            IsAdminRequest = false
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/ticketing/refunds/request", refundRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<RefundResultDto>();
        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.Contains("can only refund your own tickets", result.Message);
    }

    [Fact]
    public async Task RefundAlreadyRefundedTicket_ShouldFail()
    {
        // Arrange
        var (ticketId, visitorId) = await SeedBasicTestDataAsync();
        await CreateTestRefundAsync(ticketId, visitorId);
        
        using var factory = new TestApiFactory(fixture);
        using var client = factory.CreateClient();

        // 尝试重复退票
        var refundRequest = new RequestRefundCommand
        {
            TicketId = ticketId,
            RequestingVisitorId = visitorId,
            RefundReason = "重复退票",
            IsAdminRequest = false
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/ticketing/refunds/request", refundRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<RefundResultDto>();
        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.Contains("already been refunded", result.Message);
    }

    [Fact]
    public async Task AdminRefund_ShouldCompleteImmediately()
    {
        // Arrange
        var (ticketId, visitorId) = await SeedBasicTestDataAsync();
        
        using var factory = new TestApiFactory(fixture);
        using var client = factory.CreateClient();

        // 管理员直接退票
        var refundRequest = new RequestRefundCommand
        {
            TicketId = ticketId,
            RequestingVisitorId = visitorId,
            RefundReason = "管理员直接处理",
            IsAdminRequest = true,
            ProcessorId = 2001
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/ticketing/refunds/request", refundRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<RefundResultDto>();
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(RefundStatus.Completed, result.Status); // 管理员操作应该直接完成
        Assert.NotNull(result.ProcessedAt);

        // 验证票据状态立即更新
        var db = fixture.DbContext;
        var ticket = await db.Tickets.FindAsync(ticketId);
        Assert.NotNull(ticket);
        Assert.Equal(TicketStatus.Refunded, ticket.Status);
    }

    [Fact]
    public async Task ProcessRefund_ApproveRequest_ShouldUpdateStatusAndTicket()
    {
        // Arrange
        var (ticketId, visitorId) = await SeedBasicTestDataAsync();
        
        using var factory = new TestApiFactory(fixture);
        using var client = factory.CreateClient();

        // 先申请退票
        var refundRequest = new RequestRefundCommand
        {
            TicketId = ticketId,
            RequestingVisitorId = visitorId,
            RefundReason = "测试申请",
            IsAdminRequest = false
        };

        var refundResponse = await client.PostAsJsonAsync("/api/ticketing/refunds/request", refundRequest);
        var refundResult = await refundResponse.Content.ReadFromJsonAsync<RefundResultDto>();
        Assert.NotNull(refundResult?.RefundId);

        // 管理员处理申请
        var processRequest = new ProcessRefundCommand
        {
            RefundId = refundResult.RefundId.Value,
            Decision = RefundStatus.Approved,
            ProcessorId = 2001,
            ProcessingNotes = "批准退款"
        };

        // Act
        var processResponse = await client.PostAsJsonAsync($"/api/ticketing/refunds/{refundResult.RefundId}/process", processRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, processResponse.StatusCode);
        
        var processResult = await processResponse.Content.ReadFromJsonAsync<RefundResultDto>();
        Assert.NotNull(processResult);
        Assert.True(processResult.IsSuccess);
        Assert.Equal(RefundStatus.Completed, processResult.Status);

        // 验证票据状态已更新
        var db = fixture.DbContext;
        var updatedTicket = await db.Tickets.FindAsync(ticketId);
        Assert.NotNull(updatedTicket);
        Assert.Equal(TicketStatus.Refunded, updatedTicket.Status);
    }

    [Fact]
    public async Task GetRefundByTicketId_ShouldReturnRefundDetails()
    {
        // Arrange
        var (ticketId, visitorId) = await SeedBasicTestDataAsync();
        await CreateTestRefundAsync(ticketId, visitorId);
        
        using var factory = new TestApiFactory(fixture);
        using var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync($"/api/ticketing/refunds/ticket/{ticketId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<RefundDetailsDto>();
        Assert.NotNull(result);
        Assert.Equal(ticketId, result.TicketId);
        Assert.Equal(visitorId, result.VisitorId);
        Assert.Equal(45.00m, result.RefundAmount);
    }

    /// <summary>
    /// 创建基础测试数据（简化版本）
    /// </summary>
    private async Task<(int ticketId, int visitorId)> SeedBasicTestDataAsync()
    {
        var db = fixture.DbContext;

        // 创建基础角色和用户
        var role = new Role
        {
            RoleId = 1001,
            RoleName = "TestRole",
            IsSystemRole = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        db.Roles.Add(role);

        var adminRole = new Role
        {
            RoleId = 2001,
            RoleName = "AdminRole",
            IsSystemRole = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        db.Roles.Add(adminRole);

        var user = new User
        {
            UserId = 1001,
            Username = "testuser",
            Email = "test@example.com",
            DisplayName = "Test User",
            PasswordHash = "hashedpassword",
            RegisterTime = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            RoleId = 1001
        };
        db.Users.Add(user);

        var adminUser = new User
        {
            UserId = 2001,
            Username = "admin",
            Email = "admin@example.com",
            DisplayName = "Admin User",
            PasswordHash = "hashedpassword",
            RegisterTime = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            RoleId = 2001
        };
        db.Users.Add(adminUser);

        var visitor = new Visitor
        {
            VisitorId = 1001,
            VisitorType = VisitorType.Regular,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        db.Visitors.Add(visitor);

        var employee = new Employee
        {
            EmployeeId = 2001,
            StaffNumber = "ADMIN001",
            Position = "Administrator",
            DepartmentName = "Management",
            HireDate = DateTime.UtcNow.Date.AddYears(-1),
            EmploymentStatus = EmploymentStatus.Active,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        db.Employees.Add(employee);

        var ticketType = new TicketType
        {
            TicketTypeId = 1001,
            TypeName = "Test Ticket",
            Description = "Standard admission ticket",
            BasePrice = 50.00m,
            ApplicableCrowd = ApplicableCrowd.Adult,
            MaxSaleLimit = 1000,
            RulesText = "Standard rules",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        db.TicketTypes.Add(ticketType);

        // 创建预订
        var reservation = new Reservation
        {
            ReservationId = 1001,
            VisitorId = 1001,
            ReservationTime = DateTime.UtcNow.AddDays(-5),
            VisitDate = DateTime.UtcNow.AddDays(5),
            DiscountAmount = 0,
            TotalAmount = 50.00m,
            PaymentStatus = PaymentStatus.Paid,
            Status = ReservationStatus.Confirmed,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        db.Reservations.Add(reservation);

        // 创建预订项
        var reservationItem = new ReservationItem
        {
            ItemId = 1001,
            ReservationId = 1001,
            TicketTypeId = 1001,
            Quantity = 1,
            UnitPrice = 50.00m,
            DiscountAmount = 0,
            TotalAmount = 50.00m,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        db.ReservationItems.Add(reservationItem);

        // 创建票据
        var ticket = new Ticket
        {
            TicketId = 1001,
            ReservationItemId = 1001,
            TicketTypeId = 1001,
            VisitorId = 1001,
            SerialNumber = "TKT202501001",
            ValidFrom = DateTime.UtcNow.AddDays(-1),
            ValidTo = DateTime.UtcNow.AddDays(10),
            Status = TicketStatus.Issued,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        db.Tickets.Add(ticket);

        await db.SaveChangesAsync();

        return (1001, 1001);
    }

    /// <summary>
    /// 创建测试退款记录
    /// </summary>
    private async Task CreateTestRefundAsync(int ticketId, int visitorId)
    {
        var db = fixture.DbContext;

        var refundRecord = new RefundRecord
        {
            RefundId = 1001,
            TicketId = ticketId,
            VisitorId = visitorId,
            RefundAmount = 45.00m,
            RefundTime = DateTime.UtcNow,
            RefundReason = "Test refund",
            RefundStatus = RefundStatus.Completed,
            ProcessorId = 2001,
            ProcessingNotes = "Test processing",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        db.RefundRecords.Add(refundRecord);
        await db.SaveChangesAsync();
    }
}

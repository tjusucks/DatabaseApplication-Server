using DbApp.Application.UserSystem.Users;
using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Interfaces.UserSystem;
using MediatR;
using Moq;

namespace DbApp.Tests;

public class QueryTest
{
    [Fact]
    public void Test()
    {
        Assert.True(true, "True is true");
    }

    [Fact]
    public async Task Query_First_User_From_Frontend_To_Backend()
    {
        // Arrange
        // 创建一个模拟的用户对象
        var expectedUser = new User
        {
            UserId = 1,
            Username = "testuser",
            Email = "test@example.com",
            DisplayName = "Test User",
            PasswordHash = "hashed_password",
            RegisterTime = DateTime.UtcNow,
            RoleId = 1,
            PermissionLevel = 0
        };

        // 使用Moq创建模拟的用户仓储
        var mockUserRepository = new Mock<IUserRepository>();
        mockUserRepository.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(new List<User> { expectedUser });

        // 创建查询处理器
        var handler = new GetAllUsersQueryHandler(mockUserRepository.Object);

        // 模拟前端发起请求 - 查询所有用户
        var users = await handler.Handle(new GetAllUsersQuery(), default);

        // 获取第一条用户记录
        var firstUser = users.FirstOrDefault();

        // 打印第一条用户信息（模拟前端显示）
        if (firstUser != null)
        {
            Console.WriteLine($"First User ID: {firstUser.UserId}");
            Console.WriteLine($"First User Name: {firstUser.Username}");
            Console.WriteLine($"First User Email: {firstUser.Email}");
            Console.WriteLine($"First User Display Name: {firstUser.DisplayName}");

            // 验证获取到的用户数据
            Assert.Equal(1, firstUser.UserId);
            Assert.Equal("testuser", firstUser.Username);
            Assert.Equal("test@example.com", firstUser.Email);
            Assert.Equal("Test User", firstUser.DisplayName);
        }
        else
        {
            Console.WriteLine("No users found in the system");
            Assert.Fail("Expected at least one user in the system");
        }

        // 验证仓储方法被调用了一次
        mockUserRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
    }
}

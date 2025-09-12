using System.Security.Claims;
using DbApp.Application.UserSystem.Users;
using DbApp.Domain.Entities.UserSystem;
using DbApp.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DbApp.Presentation.Controllers;

/// <summary>
/// 用户认证控制器
/// 提供登录、注册、登出等基础认证功能
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AuthController> _logger;

    public AuthController(ApplicationDbContext context, ILogger<AuthController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// 用户登录
    /// </summary>
    /// <param name="request">登录请求</param>
    /// <returns>登录结果</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            // 基础验证
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { message = "用户名和密码不能为空" });
            }

            // 查找用户
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == request.Username);

            if (user == null)
            {
                return Unauthorized(new { message = "用户名或密码错误" });
            }

            // 简单密码验证（实际项目中应使用哈希验证）
            if (user.PasswordHash != request.Password)
            {
                return Unauthorized(new { message = "用户名或密码错误" });
            }

            // 创建用户声明
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.GivenName, user.DisplayName),
                new(ClaimTypes.Role, user.Role.RoleName),
                new("PermissionLevel", user.PermissionLevel.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = request.RememberMe,
                ExpiresUtc = request.RememberMe ? DateTimeOffset.UtcNow.AddDays(30) : DateTimeOffset.UtcNow.AddHours(8)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), authProperties);

            // 返回用户信息
            var userDto = new UserDto
            {
                UserId = user.UserId,
                Username = user.Username,
                DisplayName = user.DisplayName,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber,
                BirthDate = user.BirthDate,
                Gender = user.Gender,
                RegisterTime = user.RegisterTime,
                PermissionLevel = user.PermissionLevel,
                RoleId = user.RoleId,
                RoleName = user.Role.RoleName,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };

            return Ok(new { message = "登录成功", user = userDto });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "登录过程中发生错误");
            return StatusCode(500, new { message = "服务器内部错误" });
        }
    }

    /// <summary>
    /// 用户注册
    /// </summary>
    /// <param name="request">注册请求</param>
    /// <returns>注册结果</returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            // 基础验证
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { message = "用户名和密码不能为空" });
            }

            if (string.IsNullOrWhiteSpace(request.DisplayName))
            {
                return BadRequest(new { message = "显示名称不能为空" });
            }

            if (request.Password != request.ConfirmPassword)
            {
                return BadRequest(new { message = "两次输入的密码不一致" });
            }

            // 检查用户名是否已存在
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (existingUser != null)
            {
                return Conflict(new { message = "用户名已被使用" });
            }

            // 检查邮箱是否已存在
            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                var existingEmail = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
                if (existingEmail != null)
                {
                    return Conflict(new { message = "邮箱已被使用" });
                }
            }

            // 获取默认角色（员工角色）
            var defaultRole = await _context.Set<Role>().FirstOrDefaultAsync(r => r.RoleName == "Employee");
            if (defaultRole == null)
            {
                return StatusCode(500, new { message = "系统配置错误：未找到默认角色" });
            }

            // 创建新用户
            var newUser = new User
            {
                Username = request.Username,
                PasswordHash = request.Password, // 实际项目中应进行哈希处理
                DisplayName = request.DisplayName,
                Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email,
                PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? null : request.PhoneNumber,
                RegisterTime = DateTime.UtcNow,
                PermissionLevel = 1, // 默认权限级别
                RoleId = defaultRole.RoleId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            // 重新查询用户以获取完整信息
            var userWithRole = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == newUser.UserId);

            // 自动登录
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userWithRole!.UserId.ToString()),
                new(ClaimTypes.Name, userWithRole.Username),
                new(ClaimTypes.GivenName, userWithRole.DisplayName),
                new(ClaimTypes.Role, userWithRole.Role.RoleName),
                new("PermissionLevel", userWithRole.PermissionLevel.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            // 返回用户信息
            var userDto = new UserDto
            {
                UserId = userWithRole.UserId,
                Username = userWithRole.Username,
                DisplayName = userWithRole.DisplayName,
                Email = userWithRole.Email ?? string.Empty,
                PhoneNumber = userWithRole.PhoneNumber,
                BirthDate = userWithRole.BirthDate,
                Gender = userWithRole.Gender,
                RegisterTime = userWithRole.RegisterTime,
                PermissionLevel = userWithRole.PermissionLevel,
                RoleId = userWithRole.RoleId,
                RoleName = userWithRole.Role.RoleName,
                CreatedAt = userWithRole.CreatedAt,
                UpdatedAt = userWithRole.UpdatedAt
            };

            return Ok(new { message = "注册成功", user = userDto });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "注册过程中发生错误");
            return StatusCode(500, new { message = "服务器内部错误" });
        }
    }

    /// <summary>
    /// 用户登出
    /// </summary>
    /// <returns>登出结果</returns>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        try
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new { message = "登出成功" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "登出过程中发生错误");
            return StatusCode(500, new { message = "服务器内部错误" });
        }
    }

    /// <summary>
    /// 获取当前用户信息
    /// </summary>
    /// <returns>用户信息</returns>
    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> GetProfile()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized(new { message = "无效的用户身份" });
            }

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                return NotFound(new { message = "用户不存在" });
            }

            var userDto = new UserDto
            {
                UserId = user.UserId,
                Username = user.Username,
                DisplayName = user.DisplayName,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber,
                BirthDate = user.BirthDate,
                Gender = user.Gender,
                RegisterTime = user.RegisterTime,
                PermissionLevel = user.PermissionLevel,
                RoleId = user.RoleId,
                RoleName = user.Role.RoleName,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };

            return Ok(userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取用户信息过程中发生错误");
            return StatusCode(500, new { message = "服务器内部错误" });
        }
    }

    /// <summary>
    /// 检查用户名是否可用
    /// </summary>
    /// <param name="username">用户名</param>
    /// <returns>可用性结果</returns>
    [HttpGet("check-username/{username}")]
    public async Task<IActionResult> CheckUsername(string username)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return BadRequest(new { isAvailable = false, message = "用户名不能为空" });
            }

            var exists = await _context.Users.AnyAsync(u => u.Username == username);
            return Ok(new { isAvailable = !exists, message = exists ? "用户名已被使用" : "用户名可用" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "检查用户名可用性时发生错误");
            return StatusCode(500, new { isAvailable = false, message = "服务器内部错误" });
        }
    }
}

/// <summary>
/// 登录请求模型
/// </summary>
public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; } = false;
}

/// <summary>
/// 注册请求模型
/// </summary>
public class RegisterRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
}

# 前五功能整合指南

## 📋 文档说明

本文档专门为需要与我们的五个核心功能进行整合的开发人员编写，详细说明了每个功能的实现架构、数据流程、接口规范和扩展点，方便其他开发者理解和修改我们的功能实现。

## 🎯 功能概览

| 功能编号 | 功能名称 | 核心职责 | 主要实体 | API端点 |
|---------|----------|----------|----------|---------|
| 功能1 | 游客进出登记及人数统计 | 创建访客档案 | User, Visitor | POST /api/visitors |
| 功能2 | 游客历史信息录入 | 更新访客信息 | Visitor | PUT /api/visitors/{id} |
| 功能3 | 游客历史信息查询 | 多维度查询 | Visitor, User | GET /api/visitors/search |
| 功能4 | 会员注册登记 | 会员档案管理 | Visitor | POST /api/membership/register |
| 功能5 | 会员积分系统 | 积分和等级管理 | Visitor | POST /api/membership/points/* |

## 🏗️ 架构设计原则

### **分层架构**
```
Controller → Command/Query → Handler → Repository → Database
    ↓           ↓              ↓           ↓          ↓
  API层      应用层         业务层      数据层     存储层
```

### **设计模式**
- **CQRS**: 命令查询职责分离
- **Repository**: 数据访问抽象
- **Decorator**: 缓存装饰器
- **Transaction**: 数据库事务管理

## 🔧 功能1: 游客进出登记及人数统计

### **实现架构**
```csharp
// 命令定义
public record CreateVisitorCommand(
    string Username, string Email, string DisplayName,
    string PhoneNumber, DateTime BirthDate, Gender Gender,
    VisitorType VisitorType, int Height, string PasswordHash
) : IRequest<int>;

// 处理器实现
public class CreateVisitorCommandHandler : IRequestHandler<CreateVisitorCommand, int>
{
    // 使用数据库事务确保数据一致性
    using var transaction = await _dbContext.Database.BeginTransactionAsync();
    
    // 1. 创建User实体
    // 2. 创建Visitor实体  
    // 3. 提交事务
}
```

### **数据流程**
1. **接收请求** → `VisitorsController.CreateVisitor()`
2. **命令验证** → 参数校验和业务规则检查
3. **事务开始** → `BeginTransactionAsync()`
4. **创建User** → `UserRepository.CreateAsync()`
5. **创建Visitor** → `VisitorRepository.CreateAsync()`
6. **事务提交** → `CommitAsync()` 或 `RollbackAsync()`
7. **返回结果** → VisitorId

### **关键实现细节**
```csharp
// 事务处理模式
using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
try
{
    var userId = await _userRepository.CreateAsync(user);
    var visitor = new Visitor { VisitorId = userId, ... };
    await _visitorRepository.CreateAsync(visitor);
    await transaction.CommitAsync(cancellationToken);
    return userId;
}
catch
{
    await transaction.RollbackAsync(cancellationToken);
    throw;
}
```

### **扩展点**
- **验证规则**: 在Handler中添加自定义验证逻辑
- **事件发布**: 在创建成功后发布领域事件
- **审计日志**: 在Repository中添加操作记录

## 🔧 功能2: 游客历史信息录入

### **实现架构**
```csharp
// 命令定义
public record UpdateVisitorCommand(
    int VisitorId, string? DisplayName, string? PhoneNumber, 
    int? Height, bool? IsBlacklisted
) : IRequest<Unit>;

// 处理器实现 - 支持部分更新
public class UpdateVisitorCommandHandler : IRequestHandler<UpdateVisitorCommand, Unit>
{
    // 1. 获取现有实体
    // 2. 更新指定字段
    // 3. 保存更改
    // 4. 清除缓存
}
```

### **数据流程**
1. **接收请求** → `VisitorsController.UpdateVisitor()`
2. **实体查询** → `VisitorRepository.GetByIdAsync()`
3. **字段更新** → 仅更新非null字段
4. **数据保存** → `VisitorRepository.UpdateAsync()`
5. **缓存清理** → 自动清除相关缓存
6. **返回确认** → HTTP 204 No Content

### **关键实现细节**
```csharp
// 部分更新模式
var visitor = await _visitorRepository.GetByIdAsync(request.VisitorId);
if (request.DisplayName != null) visitor.User.DisplayName = request.DisplayName;
if (request.PhoneNumber != null) visitor.User.PhoneNumber = request.PhoneNumber;
if (request.Height.HasValue) visitor.Height = request.Height.Value;
if (request.IsBlacklisted.HasValue) visitor.IsBlacklisted = request.IsBlacklisted.Value;

visitor.UpdatedAt = DateTime.UtcNow;
await _visitorRepository.UpdateAsync(visitor);
```

### **扩展点**
- **字段验证**: 添加特定字段的验证规则
- **变更历史**: 记录字段变更历史
- **权限控制**: 基于用户角色限制可更新字段

## 🔧 功能3: 游客历史信息查询

### **实现架构**
```csharp
// 查询定义
public record SearchVisitorsQuery(
    string? Name, string? PhoneNumber, 
    VisitorType? VisitorType, bool? IsBlacklisted
) : IRequest<List<VisitorDto>>;

// 处理器实现 - 支持多条件查询
public class SearchVisitorsQueryHandler : IRequestHandler<SearchVisitorsQuery, List<VisitorDto>>
{
    // 1. 构建查询条件
    // 2. 执行数据库查询
    // 3. 转换为DTO
    // 4. 返回结果
}
```

### **数据流程**
1. **接收请求** → `VisitorsController.SearchVisitors()`
2. **条件构建** → 动态构建LINQ查询
3. **数据查询** → `VisitorRepository.SearchAsync()`
4. **结果映射** → Entity → DTO
5. **缓存存储** → 缓存查询结果
6. **返回数据** → JSON格式

### **关键实现细节**
```csharp
// 动态查询构建
var query = _dbContext.Visitors.Include(v => v.User).AsQueryable();

if (!string.IsNullOrEmpty(name))
    query = query.Where(v => v.User.Username.Contains(name) || 
                            v.User.DisplayName.Contains(name));

if (phoneNumber != null)
    query = query.Where(v => v.User.PhoneNumber.Contains(phoneNumber));

if (visitorType.HasValue)
    query = query.Where(v => v.VisitorType == visitorType.Value);

return await query.OrderBy(v => v.User.DisplayName).ToListAsync();
```

### **扩展点**
- **搜索算法**: 集成全文搜索或模糊匹配
- **分页支持**: 添加分页查询功能
- **排序选项**: 支持多字段排序

## 🔧 功能4: 会员注册登记

### **实现架构**
```csharp
// 命令定义
public record RegisterVisitorCommand(int UserId, int Height) : IRequest<int>;

// 处理器实现 - 基于现有用户创建访客
public class RegisterVisitorCommandHandler : IRequestHandler<RegisterVisitorCommand, int>
{
    // 1. 验证用户存在
    // 2. 检查访客不重复
    // 3. 创建访客记录
    // 4. 初始化会员等级
}
```

### **数据流程**
1. **接收请求** → `MembershipController.RegisterVisitor()`
2. **用户验证** → `UserRepository.GetByIdAsync()`
3. **重复检查** → `VisitorRepository.GetByUserIdAsync()`
4. **创建访客** → `VisitorRepository.CreateAsync()`
5. **等级初始化** → 默认Bronze等级
6. **返回结果** → VisitorId

### **关键实现细节**
```csharp
// 业务规则验证
var user = await _userRepository.GetByIdAsync(request.UserId)
    ?? throw new InvalidOperationException($"User with ID {request.UserId} not found");

var existingVisitor = await _visitorRepository.GetByUserIdAsync(request.UserId);
if (existingVisitor != null)
    throw new InvalidOperationException($"Visitor already exists for user ID {request.UserId}");

// 初始化访客
var visitor = new Visitor
{
    VisitorId = request.UserId,
    VisitorType = VisitorType.Regular,
    Points = 0,
    MemberLevel = MembershipConstants.LevelNames.Bronze,
    Height = request.Height,
    IsBlacklisted = false,
    CreatedAt = DateTime.UtcNow
};
```

### **扩展点**
- **注册流程**: 添加邮件确认或短信验证
- **初始奖励**: 注册时赠送初始积分
- **会员卡**: 生成物理或虚拟会员卡

## 🔧 功能5: 会员积分系统

### **实现架构**
```csharp
// 命令定义
public record AddPointsCommand(int VisitorId, int Points, string? Reason) : IRequest<Unit>;

// 处理器实现 - 积分管理和等级升级
public class AddPointsCommandHandler : IRequestHandler<AddPointsCommand, Unit>
{
    // 1. 验证访客存在
    // 2. 添加积分
    // 3. 计算新等级
    // 4. 更新数据
}
```

### **数据流程**
1. **接收请求** → `MembershipController.AddPoints()`
2. **访客验证** → `VisitorRepository.GetByIdAsync()`
3. **积分更新** → `VisitorRepository.AddPointsAsync()`
4. **等级计算** → `MembershipService.UpdateMemberLevel()`
5. **数据保存** → `VisitorRepository.UpdateAsync()`
6. **返回结果** → 包含等级变化信息

### **关键实现细节**
```csharp
// 积分和等级管理
public async Task AddPointsAsync(int visitorId, int points)
{
    var visitor = await GetByIdAsync(visitorId);
    if (visitor != null)
    {
        visitor.Points += points;
        
        // 自动等级升级
        MembershipService.UpdateMemberLevel(visitor);
        
        await UpdateAsync(visitor);
    }
}

// 等级计算算法
public static void UpdateMemberLevel(Visitor visitor)
{
    var newLevel = visitor.Points switch
    {
        >= 10000 => MembershipConstants.LevelNames.Platinum,
        >= 5000 => MembershipConstants.LevelNames.Gold,
        >= 1000 => MembershipConstants.LevelNames.Silver,
        _ => MembershipConstants.LevelNames.Bronze
    };
    
    visitor.MemberLevel = newLevel;
}

// 折扣计算
public static decimal GetDiscountMultiplier(string? memberLevel)
{
    return memberLevel switch
    {
        MembershipConstants.LevelNames.Platinum => 0.7m,  // 7折
        MembershipConstants.LevelNames.Gold => 0.8m,      // 8折  
        MembershipConstants.LevelNames.Silver => 0.9m,    // 9折
        _ => 1.0m                                          // 无折扣
    };
}
```

### **扩展点**
- **积分规则**: 支持不同活动的积分倍数
- **等级权益**: 为不同等级添加专属权益
- **积分历史**: 记录积分变更历史

## 🔄 数据库事务管理

### **事务边界原则**
1. **单一职责**: 每个事务只处理一个业务操作
2. **最小范围**: 事务范围尽可能小
3. **异常处理**: 确保异常时正确回滚
4. **超时控制**: 设置合理的事务超时时间

### **事务实现模式**
```csharp
// 标准事务模式
using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
try
{
    // 业务操作
    await DoBusinessOperation();
    
    // 提交事务
    await transaction.CommitAsync(cancellationToken);
}
catch
{
    // 回滚事务
    await transaction.RollbackAsync(cancellationToken);
    throw;
}
```

### **事务使用场景**
- **功能1**: 创建User和Visitor需要事务保护
- **功能2**: 单表更新，依赖EF Core默认事务
- **功能3**: 只读查询，无需事务
- **功能4**: 单表创建，依赖EF Core默认事务
- **功能5**: 积分更新和等级计算需要原子性

## 🔌 集成接口规范

### **命令接口**
```csharp
// 命令基类
public interface ICommand : IRequest<Unit> { }
public interface ICommand<TResponse> : IRequest<TResponse> { }

// 命令处理器基类
public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, Unit> 
    where TCommand : ICommand { }
```

### **查询接口**
```csharp
// 查询基类
public interface IQuery<TResponse> : IRequest<TResponse> { }

// 查询处理器基类
public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, TResponse> 
    where TQuery : IQuery<TResponse> { }
```

### **仓储接口**
```csharp
// 通用仓储接口
public interface IRepository<T> where T : class
{
    Task<int> CreateAsync(T entity);
    Task<T?> GetByIdAsync(int id);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}
```

## 🚀 扩展建议

### **功能扩展**
1. **事件驱动**: 添加领域事件发布机制
2. **消息队列**: 集成异步消息处理
3. **工作流**: 支持复杂业务流程
4. **规则引擎**: 动态业务规则配置

### **技术扩展**
1. **微服务**: 按功能拆分独立服务
2. **分布式事务**: 使用Saga模式
3. **读写分离**: CQRS读写数据库分离
4. **事件溯源**: 完整的事件历史记录

## 🔍 常见集成场景

### **场景1: 添加新的访客属性**
```csharp
// 1. 扩展Visitor实体
public class Visitor
{
    // 现有属性...
    public string? Nationality { get; set; }  // 新增国籍字段
    public DateTime? LastVisitDate { get; set; }  // 新增最后访问时间
}

// 2. 更新数据库迁移
dotnet ef migrations add AddVisitorNationality

// 3. 修改相关DTO
public class VisitorDto
{
    // 现有属性...
    public string? Nationality { get; set; }
    public DateTime? LastVisitDate { get; set; }
}

// 4. 更新命令和查询
public record UpdateVisitorCommand(
    int VisitorId,
    string? DisplayName,
    string? Nationality  // 新增参数
) : IRequest<Unit>;
```

### **场景2: 集成第三方支付系统**
```csharp
// 1. 定义支付接口
public interface IPaymentService
{
    Task<PaymentResult> ProcessPaymentAsync(decimal amount, string memberLevel);
}

// 2. 在积分购买中集成
public class PurchasePointsCommandHandler : IRequestHandler<PurchasePointsCommand, Unit>
{
    private readonly IPaymentService _paymentService;
    private readonly IVisitorRepository _visitorRepository;

    public async Task<Unit> Handle(PurchasePointsCommand request, CancellationToken cancellationToken)
    {
        var visitor = await _visitorRepository.GetByIdAsync(request.VisitorId);
        var discountMultiplier = MembershipService.GetDiscountMultiplier(visitor.MemberLevel);
        var finalAmount = request.Amount * discountMultiplier;

        var paymentResult = await _paymentService.ProcessPaymentAsync(finalAmount, visitor.MemberLevel);

        if (paymentResult.IsSuccess)
        {
            await _visitorRepository.AddPointsAsync(request.VisitorId, request.Points);
        }

        return Unit.Value;
    }
}
```

### **场景3: 添加访客行为分析**
```csharp
// 1. 创建行为记录实体
public class VisitorBehavior
{
    public int BehaviorId { get; set; }
    public int VisitorId { get; set; }
    public string ActionType { get; set; }  // "ENTRY", "PURCHASE", "POINTS_EARNED"
    public DateTime ActionTime { get; set; }
    public string? ActionData { get; set; }  // JSON格式的额外数据

    public Visitor Visitor { get; set; }
}

// 2. 在现有功能中添加行为记录
public class AddPointsCommandHandler : IRequestHandler<AddPointsCommand, Unit>
{
    private readonly IVisitorBehaviorRepository _behaviorRepository;

    public async Task<Unit> Handle(AddPointsCommand request, CancellationToken cancellationToken)
    {
        // 现有积分逻辑...
        await _visitorRepository.AddPointsAsync(request.VisitorId, request.Points);

        // 记录行为
        var behavior = new VisitorBehavior
        {
            VisitorId = request.VisitorId,
            ActionType = "POINTS_EARNED",
            ActionTime = DateTime.UtcNow,
            ActionData = JsonSerializer.Serialize(new { Points = request.Points, Reason = request.Reason })
        };
        await _behaviorRepository.CreateAsync(behavior);

        return Unit.Value;
    }
}
```

## 🛠️ 调试和故障排除

### **常见问题及解决方案**

#### **问题1: EF Core实体跟踪冲突**
```
错误: The instance of entity type 'Visitor' cannot be tracked because another instance with the same key value is already being tracked.

解决方案:
1. 使用 AsNoTracking() 进行只读查询
2. 使用仓储层方法避免直接操作实体
3. 确保每个操作使用独立的DbContext
```

#### **问题2: 事务死锁**
```
错误: Transaction was deadlocked on lock resources with another process

解决方案:
1. 缩短事务持续时间
2. 按固定顺序访问资源
3. 使用适当的隔离级别
4. 添加重试机制
```

#### **问题3: 缓存一致性问题**
```
错误: 缓存数据与数据库不一致

解决方案:
1. 写操作后立即清除相关缓存
2. 设置合理的缓存过期时间
3. 使用缓存标签进行批量清除
4. 考虑使用分布式缓存失效策略
```

### **性能优化建议**

#### **数据库优化**
```sql
-- 创建必要的索引
CREATE INDEX idx_visitors_points ON "visitors"("points");
CREATE INDEX idx_visitors_member_level ON "visitors"("member_level");
CREATE INDEX idx_users_email ON "users"("email");
CREATE INDEX idx_users_phone ON "users"("phone_number");

-- 查询优化
-- 避免 N+1 查询，使用 Include 预加载
var visitors = await _dbContext.Visitors
    .Include(v => v.User)
    .Where(v => v.Points > 1000)
    .ToListAsync();
```

#### **缓存策略**
```csharp
// 分层缓存策略
public class CachedVisitorRepository : IVisitorRepository
{
    private readonly IVisitorRepository _inner;
    private readonly IDistributedCache _cache;
    private readonly IMemoryCache _memoryCache;

    public async Task<Visitor?> GetByIdAsync(int visitorId)
    {
        // L1: 内存缓存 (最快)
        if (_memoryCache.TryGetValue($"visitor:{visitorId}", out Visitor? cached))
            return cached;

        // L2: 分布式缓存 (中等速度)
        var cacheKey = $"visitor:{visitorId}";
        var cachedJson = await _cache.GetStringAsync(cacheKey);
        if (cachedJson != null)
        {
            var visitor = JsonSerializer.Deserialize<Visitor>(cachedJson);
            _memoryCache.Set($"visitor:{visitorId}", visitor, TimeSpan.FromMinutes(1));
            return visitor;
        }

        // L3: 数据库 (最慢)
        var entity = await _inner.GetByIdAsync(visitorId);
        if (entity != null)
        {
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(entity),
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) });
            _memoryCache.Set($"visitor:{visitorId}", entity, TimeSpan.FromMinutes(1));
        }
        return entity;
    }
}
```

## 📊 监控和指标

### **关键性能指标 (KPI)**
```csharp
// 自定义指标收集
public class MetricsCollector
{
    private readonly IMetrics _metrics;

    public void RecordVisitorCreation(string memberLevel)
    {
        _metrics.CreateCounter("visitors_created_total")
               .WithTag("member_level", memberLevel)
               .Add(1);
    }

    public void RecordPointsTransaction(int points, string operation)
    {
        _metrics.CreateHistogram("points_transaction_amount")
               .WithTag("operation", operation)
               .Record(points);
    }

    public void RecordQueryPerformance(string queryType, double durationMs)
    {
        _metrics.CreateHistogram("query_duration_ms")
               .WithTag("query_type", queryType)
               .Record(durationMs);
    }
}
```

### **健康检查**
```csharp
// 功能健康检查
public class VisitorSystemHealthCheck : IHealthCheck
{
    private readonly IVisitorRepository _visitorRepository;
    private readonly IUserRepository _userRepository;

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            // 检查数据库连接
            var userCount = await _userRepository.GetCountAsync();
            var visitorCount = await _visitorRepository.GetCountAsync();

            // 检查关键业务指标
            if (userCount == 0)
                return HealthCheckResult.Degraded("No users found in system");

            var data = new Dictionary<string, object>
            {
                ["total_users"] = userCount,
                ["total_visitors"] = visitorCount,
                ["visitor_ratio"] = visitorCount / (double)userCount
            };

            return HealthCheckResult.Healthy("Visitor system is healthy", data);
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Visitor system is unhealthy", ex);
        }
    }
}
```

## 🔐 安全考虑

### **数据验证**
```csharp
// 输入验证器
public class CreateVisitorCommandValidator : AbstractValidator<CreateVisitorCommand>
{
    public CreateVisitorCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .Length(3, 50)
            .Matches("^[a-zA-Z0-9_]+$");

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^1[3-9]\d{9}$")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

        RuleFor(x => x.Height)
            .InclusiveBetween(50, 250);
    }
}
```

### **权限控制**
```csharp
// 基于角色的访问控制
[Authorize(Roles = "Admin,Staff")]
public class VisitorsController : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = "CanCreateVisitor")]
    public async Task<ActionResult<int>> CreateVisitor([FromBody] CreateVisitorCommand command)
    {
        return await _mediator.Send(command);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "CanUpdateVisitor")]
    public async Task<ActionResult> UpdateVisitor(int id, [FromBody] UpdateVisitorCommand command)
    {
        if (id != command.VisitorId)
            return BadRequest("ID mismatch");

        await _mediator.Send(command);
        return NoContent();
    }
}
```

---

**文档版本**: v1.0
**适用范围**: 前五功能整合
**维护团队**: 开发团队
**最后更新**: 2025-09-02

# 游乐园访客管理系统 - 功能整合指南

## 快速概览

### 核心API端点

| 功能分类 | API端点 | 说明 |
|---------|---------|------|
| **访客基础管理** | | |
| 创建访客 | `POST /api/user/visitors` | 支持可选邮箱和电话的访客创建 |
| 获取访客列表 | `GET /api/user/visitors` | 获取所有访客列表 |
| 获取单个访客 | `GET /api/user/visitors/{id}` | 根据ID获取访客详情 |
| 更新访客信息 | `PUT /api/user/visitors/{id}` | 更新访客基本信息 |
| 删除访客 | `DELETE /api/user/visitors/{id}` | 删除访客记录 |
| 更新联系信息 | `PUT /api/user/visitors/{id}/contact` | 更新访客邮箱和电话 |
| **访客查询统计** | | |
| 搜索访客 | `GET /api/user/visitors/search` | 按条件搜索访客 |
| 访客统计 | `GET /api/user/visitors/stats` | 获取访客统计数据 |
| 分组统计 | `GET /api/user/visitors/stats/grouped` | 获取分组统计数据 |
| **黑名单管理** | | |
| 加入黑名单 | `POST /api/user/visitors/{id}/blacklist` | 将访客加入黑名单 |
| 移出黑名单 | `DELETE /api/user/visitors/{id}/blacklist` | 将访客移出黑名单 |
| **会员管理** | | |
| 升级会员 | `POST /api/user/visitors/{id}/membership` | 升级为会员（需要联系信息） |
| 取消会员 | `DELETE /api/user/visitors/{id}/membership` | 取消会员资格 |
| **积分系统（仅限会员）** | | |
| ID积分加分 | `POST /api/user/visitors/{id}/points/add` | 通过访客ID给会员加分 |
| ID积分扣分 | `POST /api/user/visitors/{id}/points/deduct` | 通过访客ID给会员扣分 |
| 联系方式加分 | `POST /api/user/visitors/points/add-by-contact` | 通过邮箱或手机号给会员加分 |
| 联系方式扣分 | `POST /api/user/visitors/points/deduct-by-contact` | 通过邮箱或手机号给会员扣分 |

## 核心架构

**分层设计**: `Controller → Command/Query → Handler → Repository → Database`

**关键特性**:

- **单事务处理**: 确保数据一致性
- **RESTful API**: 标准化接口设计
- **EF Core数据种子**: 自动初始化基础数据和测试访客
- **CQRS模式**: 命令查询分离
- **可选联系信息**: 支持创建无邮箱/电话的访客
- **会员权益控制**: 仅限会员享受积分和折扣

## 基础运行流程

### **请求处理流程**

```
1. HTTP请求 → Controller
2. Controller → 创建Command/Query
3. Command/Query → MediatR分发
4. Handler → 执行业务逻辑
5. Repository → 数据库操作
6. 返回结果 → Controller → HTTP响应
```

### **典型流程示例 - 创建访客**

```
POST /api/user/visitors
    ↓
VisitorsController.CreateVisitor()
    ↓ 创建命令
CreateVisitorCommand (支持可选邮箱/电话)
    ↓ MediatR分发
CreateVisitorCommandHandler.Handle()
    ↓ 业务逻辑
创建User和Visitor实体 (导航属性)
    ↓ 数据访问
VisitorRepository.CreateAsync()
    ↓ EF Core
单事务保存到数据库
    ↓ 返回
VisitorId → Controller → JSON响应
```

## 核心功能实现

### **1. 访客创建 (支持可选联系信息)**

**运行流程**:

1. **接收请求** → `POST /api/user/visitors`
2. **参数验证** → DataAnnotations自动验证（邮箱和电话可选）
3. **创建命令** → 转换为`CreateVisitorCommand`
4. **业务处理** → Handler执行单事务创建
5. **数据持久** → EF Core保存User和Visitor
6. **返回结果** → 返回新创建的VisitorId

```csharp
// 命令定义 - 支持可选邮箱和电话
public record CreateVisitorCommand(
    string Username, string? Email, string DisplayName,
    string? PhoneNumber, DateTime BirthDate, Gender Gender,
    string PasswordHash, int Height
) : IRequest<int>;

// 处理器 - 单事务创建User和Visitor
public class CreateVisitorCommandHandler(IVisitorRepository visitorRepository)
    : IRequestHandler<CreateVisitorCommand, int>
{
    public async Task<int> Handle(CreateVisitorCommand request, CancellationToken cancellationToken)
    {
        var visitor = new Visitor
        {
            User = new User {
                Email = request.Email, // 可选字段
                PhoneNumber = request.PhoneNumber, // 可选字段
                RoleId = 1
            },
            Height = request.Height,
            VisitorType = VisitorType.Regular
        };

        await visitorRepository.CreateAsync(visitor);
        return visitor.VisitorId;
    }
}
```

**关键点**:

- 邮箱和电话号码都是可选字段
- 使用导航属性确保User和Visitor在同一事务中创建
- 新创建的访客默认为Regular类型

### **2. 会员升级 (联系信息验证)**

**运行流程**:

1. **接收请求** → `POST /api/user/visitors/{id}/membership`
2. **访客验证** → 检查访客是否存在且未被拉黑
3. **联系信息检查** → 验证是否有邮箱或电话号码
4. **升级处理** → 设置为Member类型并分配Bronze等级
5. **数据更新** → 保存会员信息和升级时间
6. **返回结果** → 返回升级成功消息

```csharp
// 命令定义 - 会员升级
public record UpgradeToMemberCommand(int VisitorId) : IRequest<Unit>;

// 处理器 - 会员升级验证
public class UpgradeToMemberCommandHandler : IRequestHandler<UpgradeToMemberCommand, Unit>
{
    public async Task<Unit> Handle(UpgradeToMemberCommand request, CancellationToken cancellationToken)
    {
        // 1. 获取访客信息
        var visitor = await _repository.GetByIdAsync(request.VisitorId);

        // 2. 验证联系信息
        if (!visitor.User.HasContactInformation())
            throw new ValidationException("Visitor must have email or phone number to become a member");

        // 3. 检查黑名单状态
        if (visitor.IsBlacklisted)
            throw new ValidationException("Blacklisted visitors cannot become members");

        // 4. 升级为会员
        visitor.VisitorType = VisitorType.Member;
        visitor.MemberLevel = "Bronze";
        visitor.MemberSince = DateTime.UtcNow;

        await _repository.UpdateAsync(visitor);
        return Unit.Value;
    }
}

// API使用示例
POST /api/user/visitors/1/membership
```

### **3. 会员积分管理 (仅限会员)**

**运行流程**:

1. **接收请求** → `POST /api/user/visitors/{id}/points/add`
2. **会员验证** → 检查访客是否为会员类型
3. **积分计算** → 更新积分总数
4. **等级评估** → 根据积分自动调整会员等级
5. **数据更新** → 保存积分和等级变更
6. **返回结果** → 返回更新后的积分和等级信息

```csharp
// 积分操作命令
public record AddPointsToVisitorCommand(int VisitorId, int Points, string? Reason) : IRequest<Unit>;
public record AddPointsByContactCommand(string? Email, string? PhoneNumber, int Points, string? Reason) : IRequest<Unit>;

// 处理器 - 仅限会员的积分管理
public class AddPointsToVisitorCommandHandler : IRequestHandler<AddPointsToVisitorCommand, Unit>
{
    public async Task<Unit> Handle(AddPointsToVisitorCommand request, CancellationToken cancellationToken)
    {
        // 1. 获取访客信息
        var visitor = await _repository.GetByIdAsync(request.VisitorId);

        // 2. 验证会员身份
        if (visitor.VisitorType != VisitorType.Member)
            throw new ValidationException("Only members can earn points");

        // 3. 更新积分
        visitor.Points += request.Points;

        // 4. 自动等级升级
        visitor.MemberLevel = CalculateMemberLevel(visitor.Points);

        // 5. 保存变更
        await _repository.UpdateAsync(visitor);
        return Unit.Value;
    }
}

// 等级自动升级逻辑
private static string CalculateMemberLevel(int points) => points switch
{
    >= 5000 => "Platinum",
    >= 3000 => "Gold",
    >= 1000 => "Silver",
    _ => "Bronze"
};
```

## 数据初始化

### **数据种子运行流程**

```
1. 应用启动 → EF Core初始化
2. OnModelCreating → 调用数据种子方法
3. 检查数据 → 如果Role表为空则插入种子数据
4. 生成迁移 → dotnet ef migrations add
5. 应用迁移 → dotnet ef database update
6. 数据就绪 → 应用程序可以正常使用RoleId=1等数据和测试访客
```

### **EF Core数据种子实现**

```csharp
// DataSeeding.cs - 数据种子定义
public static void SeedData(ModelBuilder modelBuilder)
{
    var seedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    // 种子数据：系统角色
    modelBuilder.Entity<Role>().HasData(
        new Role { RoleId = 1, RoleName = "Visitor", CreatedAt = seedDate },
        new Role { RoleId = 2, RoleName = "Employee", CreatedAt = seedDate },
        new Role { RoleId = 3, RoleName = "Manager", CreatedAt = seedDate },
        new Role { RoleId = 4, RoleName = "Admin", CreatedAt = seedDate }
    );

    // 种子数据：测试访客（5个不同类型的访客）
    // 包含2个会员（Silver、Gold）和3个普通访客
    // 涵盖不同年龄组和联系信息场景
}

// ApplicationDbContext.cs - 应用数据种子
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    DataSeeding.SeedData(modelBuilder); // 应用数据种子
    base.OnModelCreating(modelBuilder);
}
```

### **部署流程**

```bash
# 1. 创建包含数据种子的迁移
dotnet ef migrations add SeedRoleData

# 2. 应用迁移到数据库（自动插入种子数据）
dotnet ef database update

# 3. 验证数据种子
# 检查数据库中Role表是否包含4条记录
```

**关键点**:

- 数据种子在迁移中自动执行，无需手动插入
- 使用固定ID确保代码中的`RoleId = 1`等引用有效
- 种子数据只在首次迁移时插入，后续不会重复

## 与设计文档的差异对比

### **本次更新的核心变更**

#### **User实体修改：**

- `email` 字段从必需改为可选 (`string?`)
- 添加 `HasContactInformation()` 方法检查联系方式
- 添加 `IsEligibleForMemberUpgrade()` 方法验证升级条件

#### **数据库约束更新：**

- Email字段支持NULL值
- Email和PhoneNumber字段添加唯一约束（仅对非NULL值）
- 修复编译器警告和可空引用类型问题

### **业务逻辑变更**

| 功能 | 原实现 | 当前实现 |
|------|--------|----------|
| 访客创建 | 邮箱必填 | 邮箱和电话都可选 |
| 会员升级 | 无限制 | 必须有邮箱或电话其中一个 |
| 积分系统 | 所有访客可用 | 仅限会员使用 |
| 联系方式积分 | 不支持 | 支持通过邮箱/电话加分 |
| 数据种子 | 仅角色数据 | 包含5个测试访客 |

### **新增API端点**

```
PUT /api/user/visitors/{id}/contact - 更新联系信息
POST /api/user/visitors/points/add-by-contact - 通过联系方式加分
POST /api/user/visitors/points/deduct-by-contact - 通过联系方式扣分
```

### **测试覆盖**

- 新增15个单元测试用例
- 覆盖访客创建、会员升级、积分管理等核心功能
- 所有27个测试100%通过

## 集成接口

### **命令接口 (写操作)**

```csharp
// 访客管理
IRequestHandler<CreateVisitorCommand, int>
IRequestHandler<UpdateVisitorContactCommand, Unit>
IRequestHandler<UpgradeToMemberCommand, Unit>
IRequestHandler<RemoveMembershipCommand, Unit>

// 积分管理（仅限会员）
IRequestHandler<AddPointsToVisitorCommand, Unit>
IRequestHandler<DeductPointsFromVisitorCommand, Unit>
IRequestHandler<AddPointsByContactCommand, Unit>
IRequestHandler<DeductPointsByContactCommand, Unit>
```

### **查询接口 (读操作)**

```csharp
// 访客查询
IRequestHandler<GetAllVisitorsQuery, List<VisitorDto>>
IRequestHandler<GetVisitorByIdQuery, VisitorDto>

// 会员查询
IRequestHandler<GetMemberStatisticsQuery, MemberStatisticsDto>
```

## 扩展指南

### **新功能开发流程**

```
1. 需求分析 → 确定功能边界和接口
2. 设计Command/Query → 定义输入输出结构
3. 实现Handler → 编写核心业务逻辑
4. 创建Controller → 暴露HTTP API端点
5. 更新Repository → 添加必要的数据访问方法
6. 编写测试 → 单元测试和集成测试
7. 更新文档 → 添加API文档和使用示例
```

### **添加新功能的具体步骤**

1. **定义Command/Query** - 在Application层创建命令或查询
2. **实现Handler** - 编写业务逻辑处理器
3. **添加Controller** - 创建API端点
4. **更新Repository** - 如需新的数据访问方法
5. **编写测试** - 单元测试和集成测试

### **常见扩展场景**

```csharp
// 场景1: 添加访客照片功能
public record UploadVisitorPhotoCommand(int VisitorId, byte[] PhotoData) : IRequest;

// 场景2: 会员权益扩展
public record ApplyMemberDiscountCommand(int VisitorId, decimal Amount) : IRequest<decimal>;

// 场景3: 联系信息验证
public record VerifyContactInformationCommand(int VisitorId, string VerificationCode) : IRequest;
```

## 监控和健康检查

### **健康检查端点**

```
GET /health          # 应用程序健康状态
GET /health/ready    # 就绪状态检查
GET /health/live     # 存活状态检查
```

### **关键指标**

- **响应时间**: API端点平均响应时间
- **成功率**: 请求成功率 (>99%)
- **数据库连接**: 连接池状态
- **缓存命中率**: Redis缓存效率

---

## 快速开始

### **环境要求**

- .NET 9.0+
- Oracle Database
- Redis (可选，用于缓存)

### **启动步骤**

```bash
# 1. 克隆项目
git clone <repository-url>

# 2. 配置数据库连接
# 编辑 appsettings.json 中的连接字符串

# 3. 应用数据库迁移
dotnet ef database update --project src/Infrastructure --startup-project src/Presentation

# 4. 启动应用
dotnet run --project src/Presentation
```

### **测试API**

```bash
# 创建访客（支持可选邮箱和电话）
curl -X POST "http://localhost:5036/api/user/visitors" \
  -H "Content-Type: application/json" \
  -d '{"username":"testuser","email":null,"displayName":"Test User","phoneNumber":null,"birthDate":"1990-01-01","gender":0,"passwordHash":"hash","height":170}'

# 获取访客列表
curl "http://localhost:5036/api/user/visitors"

# 会员升级（需要联系信息）
curl -X POST "http://localhost:5036/api/user/visitors/1/membership"

# 会员积分（仅限会员）
curl -X POST "http://localhost:5036/api/user/visitors/1/points/add" \
  -H "Content-Type: application/json" \
  -d '{"Points":100,"Reason":"Purchase reward"}'
```

---

**文档版本**: v4.0 (访客管理系统更新版)
**最后更新**: 2025-09-06
**适用范围**: 访客管理系统功能整合和扩展指导

# 游乐园访客管理系统 - 功能整合指南

## 🎯 快速概览

| 功能 | API端点 | 核心实体 | 说明 |
|------|---------|----------|------|
| 访客创建 | `POST /api/visitors` | User, Visitor | 单事务创建用户和访客 |
| 访客更新 | `PUT /api/visitors/{id}` | Visitor | 部分字段更新 |
| 访客搜索 | `GET /api/visitors/search` | Visitor | 统一搜索端点，支持分页筛选 |
| 会员注册 | `POST /api/membership/register` | Visitor | 升级为会员 |
| 积分管理 | `POST /api/membership/points/*` | Visitor | 积分增减和等级管理 |

## 🏗️ 核心架构

**分层设计**: `Controller → Command/Query → Handler → Repository → Database`

**关键特性**:
- ✅ **单事务处理**: 确保数据一致性
- ✅ **RESTful API**: 标准化接口设计  
- ✅ **EF Core数据种子**: 自动初始化基础数据
- ✅ **CQRS模式**: 命令查询分离

## � 基础运行流程

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
POST /api/visitors
    ↓
VisitorsController.CreateVisitor()
    ↓ 创建命令
CreateVisitorCommand
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

## �🔧 核心功能实现

### **1. 访客创建 (单事务处理)**

**运行流程**:
1. **接收请求** → `POST /api/visitors`
2. **参数验证** → DataAnnotations自动验证
3. **创建命令** → 转换为`CreateVisitorCommand`
4. **业务处理** → Handler执行单事务创建
5. **数据持久** → EF Core保存User和Visitor
6. **返回结果** → 返回新创建的VisitorId

```csharp
// 命令定义
public record CreateVisitorCommand(
    string Username, string Email, string DisplayName,
    string PhoneNumber, DateTime BirthDate, Gender Gender,
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
            User = new User { /* 用户信息 */ RoleId = 1 }, // 导航属性
            Height = request.Height,
            VisitorType = VisitorType.Regular
        };

        await visitorRepository.CreateAsync(visitor); // 单事务保存
        return visitor.VisitorId;
    }
}
```

**关键点**: 使用导航属性确保User和Visitor在同一事务中创建

### **2. 访客搜索 (RESTful统一端点)**

**运行流程**:
1. **接收请求** → `GET /api/visitors/search?keyword=john&page=1`
2. **参数绑定** → 查询参数自动绑定到Query对象
3. **创建查询** → 转换为`SearchVisitorsQuery`
4. **动态筛选** → Handler构建动态LINQ查询
5. **分页查询** → 先获取总数，再获取分页数据
6. **返回结果** → 包含数据、分页信息和筛选条件

```csharp
// 查询定义 - 支持多维度搜索和分页
public record SearchVisitorsQuery(
    string? Keyword = null,           // 关键词搜索
    VisitorType? VisitorType = null,  // 类型筛选
    bool? IsBlacklisted = null,       // 黑名单筛选
    int Page = 1,                     // 页码
    int PageSize = 20                 // 页大小
) : IRequest<SearchVisitorsResult>;

// 处理器 - 动态查询构建
public class SearchVisitorsQueryHandler : IRequestHandler<SearchVisitorsQuery, SearchVisitorsResult>
{
    public async Task<SearchVisitorsResult> Handle(SearchVisitorsQuery request, CancellationToken cancellationToken)
    {
        // 1. 构建基础查询
        var query = _context.Visitors.Include(v => v.User).AsQueryable();

        // 2. 应用筛选条件
        if (!string.IsNullOrWhiteSpace(request.Keyword))
            query = query.Where(v => v.User.DisplayName.Contains(request.Keyword));

        // 3. 获取总数和分页数据
        var totalCount = await query.CountAsync();
        var visitors = await query.Skip((request.Page - 1) * request.PageSize)
                                 .Take(request.PageSize)
                                 .ToListAsync();

        return new SearchVisitorsResult { /* 结果数据 */ };
    }
}

// API使用示例
GET /api/visitors/search?keyword=john&page=1&pageSize=10
GET /api/visitors/search?visitorType=Member&isBlacklisted=false
```

### **3. 会员积分管理**

**运行流程**:
1. **接收请求** → `POST /api/membership/points/add`
2. **访客验证** → 检查访客是否存在且未被拉黑
3. **积分计算** → 更新积分总数
4. **等级评估** → 根据积分自动调整会员等级
5. **数据更新** → 保存积分和等级变更
6. **返回结果** → 返回更新后的积分和等级信息

```csharp
// 积分操作命令
public record AddPointsCommand(int VisitorId, int Points, string Reason) : IRequest;
public record DeductPointsCommand(int VisitorId, int Points, string Reason) : IRequest;

// 处理器 - 积分管理和等级升级
public class AddPointsCommandHandler : IRequestHandler<AddPointsCommand>
{
    public async Task Handle(AddPointsCommand request, CancellationToken cancellationToken)
    {
        // 1. 获取访客信息
        var visitor = await _repository.GetByIdAsync(request.VisitorId);

        // 2. 更新积分
        visitor.Points += request.Points;

        // 3. 自动等级升级
        visitor.MemberLevel = CalculateMemberLevel(visitor.Points);

        // 4. 保存变更
        await _repository.UpdateAsync(visitor);
    }
}

// 等级自动升级逻辑
private static string CalculateMemberLevel(int points) => points switch
{
    >= 10000 => "Diamond",
    >= 5000 => "Gold",
    >= 1000 => "Silver",
    _ => "Bronze"
};
```

## 🌱 数据初始化

### **数据种子运行流程**
```
1. 应用启动 → EF Core初始化
2. OnModelCreating → 调用数据种子方法
3. 检查数据 → 如果Role表为空则插入种子数据
4. 生成迁移 → dotnet ef migrations add
5. 应用迁移 → dotnet ef database update
6. 数据就绪 → 应用程序可以正常使用RoleId=1等数据
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
        new Role { RoleId = 2, RoleName = "Member", CreatedAt = seedDate },
        new Role { RoleId = 3, RoleName = "Staff", CreatedAt = seedDate },
        new Role { RoleId = 4, RoleName = "Admin", CreatedAt = seedDate }
    );
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

## � **与设计文档的差异对比**

### **设计文档中没有，但我们实现中新增的字段**

#### **`users` 表新增字段：**
- `permission_level` (NUMBER(2)) - 权限等级控制，设计文档中没有
- `register_time` (TIMESTAMP) - 注册时间，设计文档中没有
- `created_at` (TIMESTAMP) - 创建时间，设计文档中没有
- `updated_at` (TIMESTAMP) - 更新时间，设计文档中没有

#### **`visitors` 表新增字段：**
- `created_at` (TIMESTAMP) - 创建时间，设计文档中没有
- `updated_at` (TIMESTAMP) - 更新时间，设计文档中没有

#### **所有表通用新增：**
- 几乎所有表都添加了 `created_at` 和 `updated_at` 时间戳字段，用于审计追踪

### **业务逻辑差异**
| 差异项 | 原设计 | 当前实现 |
|--------|--------|----------|
| 联系方式要求 | 邮箱必填 | 邮箱或手机号二选一 |
| 会员升级条件 | 无特殊要求 | 必须有联系方式 |
| 积分系统 | 基础记录 | 自动等级升级 |
| 手机号约束 | 无唯一约束 | 添加唯一约束 |
| 数据审计 | 无时间戳 | 完整的创建/更新时间追踪 |

### **本分支新建的数据库表（29张）**
```
roles, users, visitors, blacklist, employees, staff_teams, team_members,
entry_records, tickets, ticket_types, price_rules, price_histories,
promotions, promotion_ticket_types, promotion_conditions, promotion_actions,
reservations, reservation_items, refund_records, coupons, amusement_rides,
ride_traffic_stats, maintenance_records, inspection_records, seasonal_events,
salary_records, attendances, employee_reviews, financial_records
```

## �🔌 集成接口

### **命令接口 (写操作)**
```csharp
// 访客管理
IRequestHandler<CreateVisitorCommand, int>
IRequestHandler<UpdateVisitorCommand>
IRequestHandler<BlacklistVisitorCommand>

// 会员管理  
IRequestHandler<RegisterMemberCommand>
IRequestHandler<AddPointsCommand>
IRequestHandler<DeductPointsCommand>
```

### **查询接口 (读操作)**
```csharp
// 访客查询
IRequestHandler<GetVisitorByIdQuery, VisitorDto>
IRequestHandler<SearchVisitorsQuery, SearchVisitorsResult>

// 统计查询
IRequestHandler<GetVisitorCountQuery, int>
IRequestHandler<GetMemberStatisticsQuery, MemberStatisticsDto>
```

## 🚀 扩展指南

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

// 场景2: 集成第三方支付
public record ProcessPaymentCommand(int VisitorId, decimal Amount, string PaymentMethod) : IRequest<PaymentResult>;

// 场景3: 访客行为分析
public record RecordVisitorActivityCommand(int VisitorId, string Activity, DateTime Timestamp) : IRequest;
```

## 📊 监控和健康检查

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

## 📝 快速开始

### **环境要求**
- .NET 8.0+
- Oracle Database
- Redis (可选，用于缓存)

### **启动步骤**
```bash
# 1. 克隆项目
git clone <repository-url>

# 2. 配置数据库连接
# 编辑 appsettings.json 中的连接字符串

# 3. 应用数据库迁移
dotnet ef database update

# 4. 启动应用
dotnet run --project src/Presentation
```

### **测试API**
```bash
# 创建访客
curl -X POST "https://localhost:7220/api/visitors" \
  -H "Content-Type: application/json" \
  -d '{"username":"testuser","email":"test@example.com","displayName":"Test User"}'

# 搜索访客
curl "https://localhost:7220/api/visitors/search?keyword=test&page=1&pageSize=10"
```

---

**文档版本**: v3.0 (精简版)  
**最后更新**: 2025-09-04  
**适用范围**: 快速整合和扩展指导

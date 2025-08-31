# DatabaseApplication Server

[![Ask DeepWiki](https://deepwiki.com/badge.svg)](https://deepwiki.com/tjusucks/DatabaseApplication-Server/prompt-line)

Backend API built with C# ASP.NET Core connected to Oracle Database.

## 目录结构说明

```
src/
├── Application/                # 应用层，包含业务用例、DTO、CQRS等
│   └── TicketingSystem/
│       ├── PriceRules/        # 价格规则相关CQRS与DTO
│       ├── Promotions/        # 优惠活动相关CQRS与DTO
│       └── TicketTypes/       # 票种相关CQRS与DTO
├── Domain/                    # 领域层，包含实体、枚举、仓储接口
│   └── Entities/
│   └── Enums/
│   └── Interfaces/
├── Infrastructure/            # 基础设施层，数据库上下文与仓储实现
│   └── Repositories/
├── Presentation/              # 表现层，API控制器与启动配置
│   └── Controllers/
│   └── Program.cs
└── ...
```

---

## 技术栈

- ASP.NET Core Web API
- Oracle 数据库
- Entity Framework Core
- MediatR（CQRS 模式）
- 分层架构（Clean Architecture）

---

## 主要业务功能

### 1. 票种管理

- 查询所有票种及基础价格
- 支持票种的增删改查

### 2. 价格规则管理

- 查询指定票种的所有价格规则
- 新增、修改、删除价格规则

### 3. 优惠活动管理

- 查询所有优惠活动摘要
- 查询优惠活动详情（包括适用票种、条件、动作）
- 新建、修改、删除优惠活动

---

## 枚举类型序列化说明

- 已全局配置 `JsonStringEnumConverter`。
- **请求体**：枚举字段支持数字和字符串（如 0 或 "Operating"）。
- **响应体**：枚举字段始终序列化为字符串（如 "Operating"）。

---

## 运行与开发

1. 配置数据库连接字符串于 `appsettings.json` 或环境变量。
2. 运行数据库迁移（如有）。
3. 启动项目：
   ```bash
   dotnet run --project src/Presentation
   ```
4. 通过 Swagger 或 Postman 访问 API。

---

## 贡献与扩展

- 遵循分层架构，所有业务逻辑写在 Application 层，数据访问写在 Infrastructure 层。
- 新增业务时，优先定义接口与 DTO，再实现 CQRS/服务与控制器。

---

如需详细接口文档或具体业务扩展说明，请查阅各层代码或联系维护者

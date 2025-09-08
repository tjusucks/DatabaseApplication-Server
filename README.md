# DatabaseApplication Server

[![Ask DeepWiki](https://deepwiki.com/badge.svg)](https://deepwiki.com/tjusucks/DatabaseApplication-Server/prompt-line)

Backend API built with C# ASP.NET Core connected to Oracle Database.

## 技术栈

- **ASP.NET Core Web API**：主流高性能后端框架，适合构建 RESTful API。
- **Oracle 数据库**：企业级关系型数据库，数据持久化存储。
- **Entity Framework Core**：对象关系映射（ORM）框架，简化数据库操作。
- **MediatR**：实现 CQRS（命令查询职责分离）和中介者模式，解耦请求与处理逻辑。
- **DotNetEnv**：环境变量管理，便于本地和生产环境配置切换。
- **Redis**（可选）：缓存中间件，提升数据访问性能。

---

## 设计模式

- **分层架构（Clean Architecture）**

  - **Presentation 层**：API 控制器与启动配置，负责接收请求和返回响应。
  - **Application 层**：业务用例、DTO、CQRS 命令与查询、接口定义。
  - **Domain 层**：领域实体、枚举、仓储接口，聚焦业务核心。
  - **Infrastructure 层**：数据库上下文、仓储实现、第三方服务集成。

- **CQRS（命令查询职责分离）**

  - 查询（Query）和命令（Command）分别处理，读写分离，提升可维护性和扩展性。
  - 通过 MediatR 实现请求与处理器的解耦。

- **中介者模式（Mediator Pattern）**

  - 所有命令和查询通过 MediatR 统一调度，避免直接依赖，降低耦合。

- **依赖注入（DI）**
  - 通过构造函数注入服务和仓储，便于单元测试和解耦。

---

## 开发思路

1. **领域驱动设计**
   - 以业务为核心，实体、聚合根、仓储接口均在 Domain 层定义。
2. **接口与实现分离**
   - 所有业务接口（如服务、仓储）在 Application 层定义，实现放在 Infrastructure 层。
3. **CQRS + MediatR**
   - 所有业务操作（如 PromotionCondition 的增删改）通过 Command/Query 对象和 Handler 实现，便于扩展和维护。
   - 例如：`PromotionConditionCommandHandler` 统一处理 PromotionCondition 的增删改命令。
4. **DTO 传输对象**
   - 控制器与服务、服务与仓储之间只通过 DTO 传递数据，避免直接暴露实体。
5. **全局枚举序列化规范**
   - 配置 `JsonStringEnumConverter`，API 请求体支持枚举数字和字符串，响应始终为字符串，提升前后端兼容性。
6. **异常与事务管理**
   - 关键业务操作使用事务，异常时自动回滚，保证数据一致性。

---

## 规范点

- **命名空间与目录结构一致**
  - 文件物理路径与命名空间严格对应，便于查找和维护。
- **接口命名以 I 开头，实现类与接口一一对应**
  - 如 `IPromotionRepository` 与 `PromotionRepository`。
- **DTO、实体、命令、查询、处理器分文件管理**
  - 保持单一职责，易于扩展。
- **控制器只负责路由和参数校验，不写业务逻辑**
  - 业务逻辑全部在 Application 层服务或 Handler 中实现。
- **依赖注入统一在 Program.cs 注册**
  - 保证服务生命周期和依赖关系清晰。
- **代码注释清晰，方法职责单一**
  - 便于团队协作和后期维护。
- **遵循 RESTful API 设计规范**
  - 路由、HTTP 动作、状态码语义明确。

---

## 推荐开发流程

1. 先定义领域实体和仓储接口（Domain 层）。
2. 在 Application 层定义 DTO、接口、命令/查询对象及其 Handler。
3. 在 Infrastructure 层实现仓储和服务。
4. 在 Presentation 层编写控制器，注入服务或 MediatR。
5. 编写单元测试，保证各层解耦和功能正确。
6. 通过 Swagger 或 Postman 验证 API。

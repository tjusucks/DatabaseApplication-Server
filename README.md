# DatabaseApplication Server

[![Ask DeepWiki](https://deepwiki.com/badge.svg)](https://deepwiki.com/tjusucks/DatabaseApplication-Server/prompt-line)

Backend API built with C# ASP.NET Core connected to Oracle Database.

好的，这是一个非常棒的思路！在动手写代码之前，先把业务逻辑和系统设计想清楚，可以避免大量的返工和逻辑混乱。

基于我们之前建立的数据库模型，我们来详细设计一下 **“门票价格管理”** 和 **“门票优惠活动设置”** 这两大核心功能的后端业务逻辑。

---

### 一、 总体设计思路

1.  **分层架构 (Layered Architecture)**: 我们的代码将遵循清晰的分层结构：

    - **表现层 (Presentation Layer)**: 即我们的 Web API `Controllers`。它负责接收 HTTP 请求，验证输入，并调用应用层服务。它不包含任何业务逻辑。
    - **应用层 (Application Layer)**: 即 `Services`。这是业务逻辑的核心，负责协调领域对象和数据持久化来完成一个完整的业务用例。例如 `PriceService`、`PromotionService`。
    - **领域层 (Domain Layer)**: 我们的 `Entities` 和 `Enums`。它们是业务的核心模型。
    - **基础设施层 (Infrastructure Layer)**: 我们的 `DbContext`、`Configurations` 和仓储实现，负责与数据库交互。

2.  **RESTful API 设计**: 我们将设计符合 RESTful 规范的 API 接口，使用标准的 HTTP 动词 (`GET`, `POST`, `PUT`, `DELETE`) 来表示对资源的增删改查操作。

3.  **数据传输对象 (DTOs)**: API 的输入和输出将使用 DTOs (Data Transfer Objects)。这有两个好处：
    - **解耦**: API 接口不直接暴露数据库实体（`Entities`），避免了不必要的字段泄露和循环引用问题。
    - **定制化**: DTO 可以根据前端页面的需要，精确地组合和裁剪数据。

---

### 二、 模块一：门票价格管理

这个模块的核心是管理 `TicketType` 的基础价格 (`base_price`) 和与之关联的特殊价格规则 (`PriceRule`)。

#### 1. API 端点 (Endpoints) 设计

- **`TicketTypeController`**: 管理票种本身的信息。
  - `GET /api/ticket-types`: 获取所有票种的列表（包含 ID、名称、基础价格等）。
  - `GET /api/ticket-types/{id}`: 获取单个票种的详细信息。
  - `PUT /api/ticket-types/{id}/base-price`: **【核心】** 更新一个票种的基础价格。
- **`PriceRuleController`**: 管理特殊价格规则。
  - `GET /api/ticket-types/{ticketTypeId}/price-rules`: 获取指定票种下的所有价格规则。
  - `POST /api/ticket-types/{ticketTypeId}/price-rules`: **【核心】** 为指定票种创建一个新的价格规则。
  - `PUT /api/price-rules/{ruleId}`: **【核心】** 更新一个已存在的价格规则。
  - `DELETE /api/price-rules/{ruleId}`: 删除一个价格规则。

#### 2. DTO 设计

- `TicketTypeSummaryDto { int Id, string TypeName, decimal BasePrice }`
- `UpdateBasePriceRequest { decimal NewBasePrice, string Reason, int EmployeeId }` (Reason 和 EmployeeId 用于记录价格变更历史)
- `PriceRuleDto { ... }` (包含 `PriceRule` 实体中的所有相关字段)
- `CreatePriceRuleRequest { ... }` (创建价格规则所需的字段，如 `RuleName`, `Priority`, `Price` 等)
- `UpdatePriceRuleRequest { ... }` (更新价格规则所需的字段)

#### 3. 核心业务逻辑 (`PriceService` 中实现)

- **业务逻辑 1：更新票种基础价格**

  1.  **输入**: `ticketTypeId`, `UpdateBasePriceRequest` DTO。
  2.  **验证**:
      - 检查 `ticketTypeId` 是否存在。
      - 检查 `NewBasePrice` 是否大于等于 0。
      - 检查 `EmployeeId` 是否有效。
  3.  **执行**:
      - 在一个数据库**事务 (Transaction)** 中执行以下操作：
      - a. 查询 `TicketType` 实体，获取旧价格 `oldPrice`。
      - b. 将 `TicketType` 的 `BasePrice` 更新为 `NewBasePrice`。
      - c. 创建一个新的 `PriceHistory` 实体，记录 `ticket_type_id`, `old_price`, `new_price`, `change_datetime`, `employee_id`, `reason`。
      - d. **注意**: `price_rule_id` 在这种情况下为 `null`，因为这是基础价格变更，不涉及特定规则。
      - e. 保存所有更改到数据库。
  4.  **输出**: 返回成功或失败的结果。

- **业务逻辑 2：创建/更新价格规则**
  1.  **输入**: `CreatePriceRuleRequest` 或 `UpdatePriceRuleRequest` DTO。
  2.  **验证**:
      - 检查关联的 `ticketTypeId` 是否存在。
      - 检查 `EffectiveStartDate` 是否早于 `EffectiveEndDate`。
      - 检查 `Price` 是否大于等于 0。
      - 检查 `Priority` 的值是否合理。
      - 【可选】检查同一票种下，是否存在时间段重叠且优先级相同的规则，以避免冲突。
  3.  **执行**:
      - 创建一个新的 `PriceRule` 实体或更新已有的实体。
      - 将 DTO 中的数据映射到实体属性上。
      - 保存到数据库。
  4.  **输出**: 返回新创建或更新后的 `PriceRuleDto`。

---

### 三、 模块二：门票优惠活动设置

这个模块要复杂得多，因为它涉及多个表的联动：`Promotion` (主表), `PromotionTicketType` (适用票种), `PromotionCondition` (条件), `PromotionAction` (动作)。

#### 1. API 端点 (Endpoints) 设计

- **`PromotionController`**: 管理整个优惠活动。
  - `GET /api/promotions`: 获取所有优惠活动的列表（简要信息）。
  - `POST /api/promotions`: **【核心】** 创建一个全新的、完整的优惠活动（可能包含适用票种、条件、动作）。
  - `GET /api/promotions/{id}`: 获取一个优惠活动的完整详细信息（包含所有关联信息）。
  - `PUT /api/promotions/{id}`: 更新一个优惠活动的基本信息（如名称、时间、状态等）。
  - `DELETE /api/promotions/{id}`: 删除一个优惠活动（及其所有关联的条件和动作）。

#### 2. DTO 设计

这将是 DTO 设计最复杂的部分：

- `PromotionSummaryDto { int Id, string Name, string Type, DateTime StartDate, DateTime EndDate, bool IsActive }`
- `PromotionDetailDto { ... }` (包含活动基本信息，以及 `List<TicketTypeSummaryDto> ApplicableTickets`, `List<PromotionConditionDto> Conditions`, `List<PromotionActionDto> Actions`)
- **`CreatePromotionRequest`**: 这是一个大的复合 DTO，前端可能在一个表单里提交所有信息。

  ```csharp
  class CreatePromotionRequest {
      // Promotion 基本信息
      public string PromotionName { get; set; }
      public PromotionType PromotionType { get; set; }
      // ... 其他基本字段

      // 适用票种 (ID列表)
      public List<int> ApplicableTicketTypeIds { get; set; }

      // 条件列表
      public List<CreateConditionRequest> Conditions { get; set; }

      // 动作列表
      public List<CreateActionRequest> Actions { get; set; }
  }
  ```

- `CreateConditionRequest { ... }` (包含创建 `PromotionCondition` 所需的所有可空字段)
- `CreateActionRequest { ... }` (包含创建 `PromotionAction` 所需的所有可空字段)

#### 3. 核心业务逻辑 (`PromotionService` 中实现)

- **业务逻辑 3：创建一个完整的优惠活动**

  1.  **输入**: `CreatePromotionRequest` 复合 DTO。
  2.  **验证**:
      - 验证活动基本信息（如时间、名称等）。
      - 验证 `ApplicableTicketTypeIds` 中的所有 ID 都是有效的 `TicketType` ID。
      - 循环验证 `Conditions` 列表中的每个条件对象的数据是否合法（例如，`ConditionType` 为 `MinQuantity` 时，`MinQuantity` 字段必须有值）。
      - 循环验证 `Actions` 列表中的每个动作对象的数据是否合法（例如，`ActionType` 为 `PercentOff` 时，`DiscountPercentage` 必须有值且在 0-100 之间）。
  3.  **执行**:
      - 在一个数据库**事务 (Transaction)** 中执行所有操作：
      - a. 创建 `Promotion` 主实体并保存，以获取 `promotion_id`。
      - b. 遍历 `ApplicableTicketTypeIds`，为每个 ID 创建一条 `PromotionTicketType` 关联记录。
      - c. 遍历 `Conditions` DTO 列表，为每个 DTO 创建一个 `PromotionCondition` 实体，并设置其 `promotion_id`。
      - d. 遍历 `Actions` DTO 列表，为每个 DTO 创建一个 `PromotionAction` 实体，并设置其 `promotion_id`。
      - e. 一次性将所有新建的关联记录和实体保存到数据库。
  4.  **输出**: 返回新创建的优惠活动的 `PromotionDetailDto`。

- **业务逻辑 4：获取优惠活动详情**
  1.  **输入**: `promotionId`。
  2.  **执行**:
      - 查询 `Promotion` 实体。
      - 使用 `Include()` 或 `ThenInclude()` 预加载（Eager Loading）其关联的 `PromotionTicketTypes` (以及其下的`TicketType`)、`Conditions` 和 `Actions`。
      - 将查询出的复杂实体树，映射（Map）到 `PromotionDetailDto`。
  3.  **输出**: 返回填充好数据的 `PromotionDetailDto`。

### 四、 我们要写些什么 (总结)

1.  **Controllers**:

    - `TicketTypeController.cs`
    - `PriceRuleController.cs`
    - `PromotionController.cs`

2.  **DTOs**:

    - 在 `Presentation` 层的 `Models` 或专门的 `DTOs` 文件夹下，创建上述所有 DTO 类。

3.  **Services (应用层)**:

    - 定义接口 `IPriceService` 和 `IPromotionService`。
    - 创建实现类 `PriceService.cs` 和 `PromotionService.cs`，在里面编写上述的核心业务逻辑。

4.  **依赖注入 (Dependency Injection)**:
    - 在 `Program.cs` (或 `Startup.cs`) 中，将 `DbContext`、`PriceService`、`PromotionService` 注册到依赖注入容器中。

这个设计蓝图清晰地定义了我们需要创建的文件、类、方法以及它们之间的交互方式，为下一步的具体编码实现铺平了道路。

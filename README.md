# DatabaseApplication Server

[![Ask DeepWiki](https://deepwiki.com/badge.svg)](https://deepwiki.com/tjusucks/DatabaseApplication-Server/prompt-line)

Backend API built with C# ASP.NET Core connected to Oracle Database.

### 一、 后端核心业务功能梳理 (`Services` 层)

#### 模块一：价格管理 (`PriceService`)

**核心职责**: 负责管理所有与票价相关的业务，包括基础价格和动态价格规则。

**具体功能点与业务逻辑**:

1.  **获取票种信息 (`GetAllTicketTypesAsync`, `GetTicketTypeByIdAsync`)**

    - **业务逻辑**: 提供查询所有票种或单个票种的摘要信息（ID, 名称, 基础价格）的接口。这是价格管理和后续优惠活动设置的基础。

2.  **更新票种基础价格 (`UpdateBasePriceAsync`)**

    - **业务逻辑**: 这是一个带审计功能的核心操作。
      - **事务性**: 操作被包裹在数据库事务中，确保数据一致性。
      - **验证**: 检查票种和操作员工是否存在，且新价格必须为非负数。
      - **核心操作**: 更新 `TicketTypes` 表中的 `BasePrice` 字段。
      - **审计日志**: 同时，在 `PriceHistories` 表中创建一条新的变更记录，包含旧价格、新价格、变更时间、操作员工 ID 和变更原因。

3.  **获取特定票种的价格规则 (`GetPriceRulesAsync`)**

    - **业务逻辑**: 查询并返回某个特定票种下所有已配置的特殊价格规则列表。

4.  **创建价格规则 (`CreatePriceRuleAsync`)**

    - **业务逻辑**:
      - **验证**: 检查所属票种是否存在，价格是否为非负数，以及生效开始日期必须早于结束日期。
      - **核心操作**: 在 `PriceRules` 表中创建一条新记录，关联到指定的票种。

5.  **更新价格规则 (`UpdatePriceRuleAsync`)**

    - **业务逻辑**:
      - **验证**: 同创建逻辑，确保规则存在且输入数据有效。
      - **核心操作**: 更新 `PriceRules` 表中指定规则的字段。

6.  **删除价格规则 (`DeletePriceRuleAsync`)**
    - **业务逻辑**:
      - **验证**: 检查规则是否存在。
      - **核心操作**: 从 `PriceRules` 表中删除指定的记录。

---

#### 模块二：优惠活动管理 (`PromotionService`)

**核心职责**: 负责创建和管理复杂的营销优惠活动，包括活动的适用范围、触发条件和具体优惠动作。

**具体功能点与业务逻辑**:

1.  **获取优惠活动列表 (`GetAllPromotionsAsync`)**

    - **业务逻辑**: 查询并返回系统中所有优惠活动的摘要信息（ID, 名称, 类型, 时间, 是否激活）。

2.  **获取优惠活动完整详情 (`GetPromotionDetailAsync`)**

    - **业务逻辑**:
      - **数据聚合**: 这是一个只读但复杂的数据查询操作。
      - **预加载 (Eager Loading)**: 使用 `Include`/`ThenInclude` 一次性加载活动主信息及其所有关联的子项：适用的票种 (`PromotionTicketTypes`)、触发条件 (`PromotionConditions`) 和优惠动作 (`PromotionActions`)。
      - **数据转换**: 将从数据库查出的、复杂的实体对象树，映射为结构清晰的 `PromotionDetailDto`，方便前端展示。

3.  **创建完整的优惠活动 (`CreatePromotionAsync`)**

    - **业务逻辑**: 这是系统中最复杂的写入操作之一。
      - **事务性**: 整个创建过程被包裹在数据库事务中。
      - **核心操作**:
        1.  基于输入的 DTO 创建 `Promotion` 主记录。
        2.  根据 `ApplicableTicketTypeIds` 列表，在 `PromotionTicketTypes` 关联表中创建多条记录。
        3.  遍历 `Conditions` DTO 列表，批量创建 `PromotionConditions` 记录，并与主活动关联。
        4.  遍历 `Actions` DTO 列表，批量创建 `PromotionActions` 记录，并与主活动关联。
      - **原子性**: 所有记录要么全部创建成功，要么在出现任何错误时全部回滚，不留脏数据。

4.  **更新优惠活动 (`UpdatePromotionAsync`)**

    - **业务逻辑**:
      - **当前范围**: 目前只支持更新 `Promotion` 主表的基本信息（如名称、时间、激活状态）。
      - **未来扩展**: 子项（适用票种、条件、动作）的修改，建议通过独立的、更细粒度的 API 接口来实现，以降低逻辑复杂度。

5.  **删除优惠活动 (`DeletePromotionAsync`)**
    - **业务逻辑**:
      - **级联删除**: 删除一个优惠活动时，必须同时删除其所有相关的子记录。
      - **预加载**: 先使用 `Include` 加载所有关联的子项。
      - **核心操作**: 依次使用 `RemoveRange` 删除所有子项，最后再删除 `Promotion` 主记录，确保没有孤立数据残留。

---

### 二、 项目文件结构组织原因 (架构解释)

我们当前的项目结构遵循了**整洁架构 (Clean Architecture)** 或类似的**分层架构**思想。将代码这样组织，是为了实现**高内聚、低耦合**，让项目易于理解、测试、维护和扩展。

#### 1. `Domain` (领域层) - **业务核心**

- **放什么**: 实体 (`Entities`) 和枚举 (`Enums`)。
- **为什么**: 这是我们业务的核心规则和数据结构，是我们整个系统的“语言”。它应该是最稳定、最纯粹的部分，不应该知道任何关于数据库、Web API 或其他技术细节的事情。**它不依赖任何其他层**。

#### 2. `Application` (应用层) - **业务流程**

- **放什么**:
  - **服务接口 (`Interfaces/Services`)**: 如 `IPriceService`。它们定义了我们的系统能做什么（业务用例）。
  - **服务实现 (`Features/.../Services`)**: 如 `PriceService`。它们编排领域对象来执行具体的业务逻辑。
  - **仓储接口 (`Interfaces/Persistence`)**: 如 `IPriceRuleRepository`。它们定义了业务逻辑需要对数据进行哪些操作（增、删、改、查），但**不关心如何实现**。
  - **数据传输对象 (`DTOs`)**: 定义了应用层与外界（如 `Presentation` 层）沟通的数据格式。
- **为什么**:
  - **核心逻辑的封装**: `Application` 层封装了所有的业务流程。这使得我们的业务逻辑独立于任何具体的 UI 或数据库技术。
  - **依赖倒置**: 通过定义接口（特别是仓储接口），`Application` 层不直接依赖 `Infrastructure` 层，而是 `Infrastructure` 层反过来依赖 `Application` 层的接口。这实现了“控制反转”，是实现低耦合的关键。

#### 3. `Infrastructure` (基础设施层) - **技术实现**

- **放什么**:
  - **数据库上下文 (`DbContext`)**: Entity Framework Core 的核心。
  - **数据库配置 (`Configurations`)**: Fluent API 配置。
  - **仓储实现 (`Repositories`)**: 如 `PriceRepository`。这里是**实现** `Application` 层定义的仓储接口的地方，包含了所有具体的数据库查询代码 (`_dbContext.PriceRules.Where(...)` 等)。
  - **其他外部服务**: 如邮件服务、文件存储服务、支付网关等的具体实现。
- **为什么**:
  - **隔离技术细节**: 所有与外部世界（数据库、文件系统、第三方 API）打交道的“脏活累活”都被隔离在这一层。
  - **可替换性**: 因为 `Infrastructure` 层只是 `Application` 层接口的具体实现，所以我们可以轻松地替换它。例如，明天想把 Oracle 数据库换成 SQL Server，我们只需要在 `Infrastructure` 层写一套新的 `Repository` 实现和一个新的 `DbContext`，而 `Application` 层和 `Presentation` 层的代码**一行都不用改**。

#### 4. `Presentation` (表现层) - **用户入口**

- **放什么**:
  - **API 控制器 (`Controllers`)**: 接收 HTTP 请求，调用 `Application` 层的服务，并返回结果。
  - **中间件 (`Middlewares`)**: 如全局异常处理、认证等。
- **为什么**:
  - **职责单一**: 这一层非常“薄”，它只负责与 HTTP 协议打交道，不包含任何业务逻辑。它的工作就是当好“传话筒”和“保安”。
  - **解耦**: UI 和业务逻辑分离。我们可以为同一个 `Application` 层搭配不同的 `Presentation` 层，比如一个 Web API、一个 gRPC 服务、一个桌面应用等。

## **总结**: 这种结构就像建造一栋大楼，`Domain` 是建筑蓝图，`Application` 是主体框架和功能区规划，`Infrastructure` 是水电、暖通等具体的工程实现，`Presentation` 则是大楼的入口和大堂。每一部分各司其职，相互之间通过标准的接口（门、插座）连接，而不是把电线和水管胡乱地缠绕在一起。

# 后端 API 调用指南

本项目前端与后端通过 RESTful API 进行通信。推荐使用 `axios` 库进行网络请求。

## 1. 基础配置

建议在 `src/api/index.js` 中创建一个 `axios` 实例，用于统一配置 API 的 `baseURL`、`headers` 和全局错误处理。

```javascript
import axios from "axios";

// 创建一个 axios 实例
const apiClient = axios.create({
  // 从环境变量中读取 API 的基础 URL
  // 在 .env.development 文件中可以设置 VUE_APP_API_BASE_URL = https://localhost:7123
  baseURL: process.env.VUE_APP_API_BASE_URL || "https://localhost:7123/api",
  headers: {
    "Content-Type": "application/json",
    // 如果有认证（如 JWT），可以在这里统一设置
    // 'Authorization': `Bearer ${token}`
  },
});

// 你可以添加请求和响应拦截器来处理全局的 loading 状态或错误
apiClient.interceptors.response.use(
  (response) => response.data, // 直接返回 response.data，简化后续操作
  (error) => {
    // 全局错误处理
    console.error("API Error:", error.response);
    // 可以显示一个全局的错误提示
    // alert(error.response.data.message || '请求失败');
    return Promise.reject(error);
  }
);

export default apiClient;
```

## 2. API 服务模块

为了方便管理，我们将 API 请求按功能模块划分。

### 价格服务 (`src/api/priceService.js`)

```javascript
import apiClient from "./index";

export const priceService = {
  // GET /api/TicketType
  getAllTicketTypes() {
    return apiClient.get("/TicketType");
  },

  // PUT /api/TicketType/{id}/base-price
  updateBasePrice(ticketTypeId, data) {
    // data 应该是一个对象: { newBasePrice, reason, employeeId }
    return apiClient.put(`/TicketType/${ticketTypeId}/base-price`, data);
  },

  // GET /api/ticket-types/{ticketTypeId}/price-rules
  getPriceRulesByTicketType(ticketTypeId) {
    return apiClient.get(`/ticket-types/${ticketTypeId}/price-rules`);
  },

  // POST /api/ticket-types/{ticketTypeId}/price-rules
  createPriceRule(ticketTypeId, data) {
    // data 应该是一个 CreatePriceRuleRequest 对象
    return apiClient.post(`/ticket-types/${ticketTypeId}/price-rules`, data);
  },
};
```

### 优惠活动服务 (`src/api/promotionService.js`)

```javascript
import apiClient from "./index";

export const promotionService = {
  // GET /api/Promotion
  getAllPromotions() {
    return apiClient.get("/Promotion");
  },

  // GET /api/Promotion/{id}
  getPromotionDetail(id) {
    return apiClient.get(`/Promotion/${id}`);
  },

  // POST /api/Promotion
  createPromotion(data) {
    // data 应该是一个 CreatePromotionRequest 对象
    return apiClient.post("/Promotion", data);
  },
};
```

## 3. 在 Vue 组件中调用

### 示例：获取票种列表 (`GET`)

```vue
<template>
  <div>
    <h1>票种列表</h1>
    <ul v-if="ticketTypes.length">
      <li v-for="ticket in ticketTypes" :key="ticket.id">
        {{ ticket.typeName }} - ￥{{ ticket.basePrice }}
      </li>
    </ul>
    <p v-else>正在加载...</p>
  </div>
</template>

<script setup>
import { ref, onMounted } from "vue";
import { priceService } from "@/api/priceService";

const ticketTypes = ref([]);

onMounted(async () => {
  try {
    // 调用 API 服务
    const data = await priceService.getAllTicketTypes();
    ticketTypes.value = data;
  } catch (error) {
    console.error("获取票种列表失败:", error);
  }
});
</script>
```

### 示例：创建优惠活动 (`POST`)

```vue
<template>
  <form @submit.prevent="handleSubmit">
    <!-- 这里是各种表单输入项，v-model 绑定到 promotionData -->
    <input v-model="promotionData.promotionName" placeholder="活动名称" />
    <!-- ... 其他表单项 ... -->
    <button type="submit">创建活动</button>
  </form>
</template>

<script setup>
import { ref } from "vue";
import { promotionService } from "@/api/promotionService";

// 使用 ref 来创建一个响应式的表单数据对象
const promotionData = ref({
  promotionName: "新春特惠",
  promotionType: "FullReduction",
  startDate: "2025-01-20T00:00:00Z",
  endDate: "2025-02-10T23:59:59Z",
  applicableTicketTypeIds: [1, 2], // 假设ID为1和2的票种适用
  conditions: [
    {
      conditionName: "满100元",
      conditionType: "MinAmount",
      minAmount: 100,
      priority: 10,
    },
  ],
  actions: [
    {
      actionName: "减15元",
      actionType: "AmountOff",
      discountAmount: 15,
    },
  ],
});

const handleSubmit = async () => {
  try {
    // 将整个 promotionData 对象作为请求体发送
    const createdPromotion = await promotionService.createPromotion(
      promotionData.value
    );
    alert(
      `活动 "${createdPromotion.name}" 创建成功！ID: ${createdPromotion.id}`
    );
    // 可以在这里做一些跳转或清空表单的操作
  } catch (error) {
    console.error("创建活动失败:", error);
    alert("创建活动失败，请查看控制台获取详情。");
  }
};
</script>
```

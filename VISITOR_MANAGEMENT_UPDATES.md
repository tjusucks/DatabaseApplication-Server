# 访客管理系统更新说明

## 📋 更新概述

本次更新实现了以下核心需求：

1. **访客创建API** - 允许创建没有邮箱和电话的访客
2. **会员升级限制** - 要成为会员必须要有电话或者邮箱的其中一个
3. **会员权益控制** - 只有成为会员才能享受各种会员权益
4. **联系信息更新** - 新增更新访客联系信息的功能

## 🔧 主要修改

### 1. 数据模型修改

#### User实体 (`src/Domain/Entities/UserSystem/User.cs`)
- **Email字段**：从必需字段改为可选字段 (`string?`)
- **新增方法**：
  - `HasContactInformation()` - 检查是否有联系方式
  - `IsEligibleForMemberUpgrade()` - 检查是否符合会员升级条件

### 2. 命令和查询修改

#### CreateVisitorCommand (`src/Application/UserSystem/Visitors/VisitorCommands.cs`)
- **Email参数**：从必需 (`string`) 改为可选 (`string?`)
- **新增命令**：`UpdateVisitorContactCommand` - 更新联系信息

#### VisitorCommandHandlers (`src/Application/UserSystem/Visitors/VisitorCommandHandlers.cs`)
- **CreateVisitorCommand处理**：移除邮箱和电话的必需验证
- **UpgradeToMemberCommand处理**：添加联系信息验证和黑名单检查
- **新增处理器**：`UpdateVisitorContactCommand` - 处理联系信息更新
- **RemoveMembershipCommand处理**：正确设置VisitorType为Regular

### 3. 业务逻辑服务

#### MembershipService (`src/Application/UserSystem/Visitors/Services/MembershipService.cs`)
- **会员权益控制**：只有Member类型的访客才能享受折扣
- **会员升级验证**：要求至少有一种联系方式
- **折扣计算**：Regular访客无折扣，Member根据等级享受折扣

### 4. API控制器更新

#### VisitorsController (`src/Presentation/Controllers/UserSystem/VisitorsController.cs`)
- **新增端点**：`PUT /api/user/visitors/{id}/contact` - 更新联系信息
- **错误处理**：为会员升级和联系信息更新添加适当的错误处理

## 🧪 测试覆盖

### 新增测试文件
- `tests/Unit/UserSystem/VisitorCreationTests.cs` - 15个测试用例

### 测试覆盖范围
1. **访客创建**：
   - 无邮箱和电话的访客创建
   - 仅有邮箱的访客创建
   - 仅有电话的访客创建

2. **联系信息验证**：
   - 各种联系信息组合的验证
   - 空字符串和null值的处理

3. **会员升级**：
   - 有联系信息的升级成功
   - 无联系信息的升级失败

4. **会员权益**：
   - Regular访客无折扣
   - 不同等级Member的折扣计算

## 📊 测试结果

- **总测试数量**：27个
- **通过率**：100%
- **新增测试**：15个
- **运行时间**：约13秒

## 🔄 业务流程

### 访客注册流程
```
1. 创建访客 (无需邮箱/电话) → Regular访客
2. 添加联系信息 (可选)
3. 申请会员升级 → 验证联系信息 → Member访客
4. 享受会员权益 (折扣、积分等)
```

### 会员权益等级
- **Bronze会员**：无折扣 (1.0倍)
- **Silver会员**：9折 (0.9倍)
- **Gold会员**：8折 (0.8倍)
- **Platinum会员**：7折 (0.7倍)

## 🚀 API端点

### 新增端点
```http
PUT /api/user/visitors/{id}/contact
Content-Type: application/json

{
  "visitorId": 1,
  "email": "new@example.com",
  "phoneNumber": "1234567890"
}
```

### 修改端点
```http
POST /api/user/visitors
Content-Type: application/json

{
  "username": "testuser",
  "passwordHash": "hashedpassword",
  "email": null,  // 现在可选
  "displayName": "Test User",
  "phoneNumber": null,  // 现在可选
  "visitorType": "Regular",
  "height": 170
}
```

## ⚠️ 重要注意事项

1. **向后兼容性**：现有的访客数据不受影响
2. **会员降级**：移除会员资格时会重置为Regular类型
3. **联系信息保护**：Member访客必须保持至少一种联系方式
4. **数据验证**：所有输入都经过适当的验证和清理

## 🔧 技术实现细节

- **Clean Architecture**：遵循领域驱动设计原则
- **CQRS模式**：命令和查询分离
- **依赖注入**：使用MediatR进行命令处理
- **异常处理**：统一的异常处理和错误响应
- **单元测试**：全面的测试覆盖

## 📝 后续建议

1. 考虑添加邮箱/电话验证功能
2. 实现会员等级自动升级机制
3. 添加会员权益使用记录
4. 考虑实现批量联系信息更新功能

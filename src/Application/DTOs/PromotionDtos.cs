

// 在文件顶部添加这个 using，以引入 PromotionType 等枚举
using DbApp.Domain.Enums.TicketingSystem;


namespace DbApp.Application.DTOs
{
    // --- 用于 GET 列表的简要 DTO ---
    public class PromotionSummaryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // 初始化以避免 nullable 警告
        public string Type { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }

    // --- 用于 GET 单个详情的复合 DTO ---
    public class PromotionDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }

        // REVISED: 初始化 List 以避免 nullable 警告
        public List<TicketTypeSummaryDto> ApplicableTickets { get; set; } = new();
        public List<PromotionConditionDto> Conditions { get; set; } = new();
        public List<PromotionActionDto> Actions { get; set; } = new();
    }

    // --- 用于创建 Promotion 的复合 Request DTO ---
    public class CreatePromotionRequest
    {
        public string PromotionName { get; set; }

        public PromotionType PromotionType { get; set; }

        // *** ADDED: 你的 Service 逻辑需要这两个字段，但 DTO 中缺失了 ***
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // REVISED: 初始化 List 以避免 nullable 警告
        public List<int> ApplicableTicketTypeIds { get; set; } = new();
        public List<CreateConditionRequest> Conditions { get; set; } = new();
        public List<CreateActionRequest> Actions { get; set; } = new();
    }


    // *** 以下是原来文件中缺失的 DTO 定义 ***

    // --- PromotionCondition 的 DTO ---
    public class PromotionConditionDto
    {
        public int ConditionId { get; set; }
        public string ConditionName { get; set; } = string.Empty;
        public ConditionType ConditionType { get; set; } // 注意：枚举名是 ConditionType
        public int? TicketTypeId { get; set; }
        public int? MinQuantity { get; set; }
        public decimal? MinAmount { get; set; }
        public int Priority { get; set; }
        // ... 其他你希望返回给前端的字段
    }

    // --- PromotionAction 的 DTO ---
    public class PromotionActionDto
    {
        public int ActionId { get; set; }
        public string ActionName { get; set; } = string.Empty;
        public PromotionActionType ActionType { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal? DiscountAmount { get; set; }
        // ... 其他你希望返回给前端的字段
    }

    // --- 创建 Condition 的 Request DTO ---
    public class CreateConditionRequest
    {
        public string ConditionName { get; set; }
        public ConditionType ConditionType { get; set; }
        public int? TicketTypeId { get; set; }
        public int? MinQuantity { get; set; }
        public decimal? MinAmount { get; set; }
        public int Priority { get; set; }
        // ... 其他创建时需要的字段
    }

    // --- 创建 Action 的 Request DTO ---
    public class CreateActionRequest
    {
        public string ActionName { get; set; }
        public PromotionActionType ActionType { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal? DiscountAmount { get; set; }
        // ... 其他创建时需要的字段
    }


    public class UpdatePromotionRequest
    {
        public string PromotionName { get; set; }
        public PromotionType PromotionType { get; set; } 
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}
using DbApp.Application.TicketingSystem.TicketTypes;
using DbApp.Domain.Enums.TicketingSystem;

namespace DbApp.Application.TicketingSystem.Promotions;

public class PromotionDto
{
    public int PromotionId { get; set; }
    public string PromotionName { get; set; } = string.Empty;
    public PromotionType PromotionType { get; set; }
    public string? Description { get; set; }
    public DateTime StartDatetime { get; set; }
    public DateTime EndDatetime { get; set; }
    public int? UsageLimitPerUser { get; set; }
    public int? TotalUsageLimit { get; set; }
    public int CurrentUsageCount { get; set; }
    public int DisplayPriority { get; set; }
    public bool AppliesToAllTickets { get; set; }
    public bool IsActive { get; set; }
    public bool IsCombinable { get; set; }
    public int EmployeeId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

// Detail DTO including navigation properties.
public class PromotionDetailDto : PromotionDto
{
    public List<TicketTypeDto> TicketTypes { get; set; } = [];
#pragma warning disable S125
    // TODO: Define PromotionCondition and PromotionAction DTOs.
    // public List<PromotionConditionDto> Conditions { get; set; } = [];
    // public List<PromotionActionDto> Actions { get; set; } = [];
#pragma warning restore S125

}

public class PromotionConditionDto
{
    public int ConditionId { get; set; }
    public string ConditionName { get; set; } = string.Empty;
    public ConditionType ConditionType { get; set; }
    public int? TicketTypeId { get; set; }
    public int? MinQuantity { get; set; }
    public decimal? MinAmount { get; set; }
    public int Priority { get; set; }
}

public class PromotionActionDto
{
    public int ActionId { get; set; }
    public string ActionName { get; set; } = string.Empty;
    public PromotionActionType ActionType { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public decimal? DiscountAmount { get; set; }
}

public class CreateConditionRequest
{
    public string ConditionName { get; set; } = string.Empty;
    public ConditionType ConditionType { get; set; }
    public int? TicketTypeId { get; set; }
    public int? MinQuantity { get; set; }
    public decimal? MinAmount { get; set; }
    public int Priority { get; set; }
}

public class CreateActionRequest
{
    public string ActionName { get; set; } = string.Empty;
    public PromotionActionType ActionType { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public decimal? DiscountAmount { get; set; }
}

// ***************************************************************
// ***                     补全内容结束                      ***
// ***************************************************************


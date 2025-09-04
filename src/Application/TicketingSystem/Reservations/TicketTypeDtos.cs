using DbApp.Domain.Enums.TicketingSystem;

namespace DbApp.Application.TicketingSystem.Reservations;

/// <summary>
/// 票种信息DTO
/// </summary>
public class TicketTypeDto
{
    public int TicketTypeId { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal BasePrice { get; set; }
    public string? RulesText { get; set; }
    public int? MaxSaleLimit { get; set; }
    public ApplicableCrowd ApplicableCrowd { get; set; }
    public bool IsAvailable { get; set; } = true;
    public int RemainingQuantity { get; set; } = int.MaxValue;
    public bool IsActive => IsAvailable;
}

/// <summary>
/// 预订项目请求DTO
/// </summary>
public class ReservationItemRequestDto
{
    public int TicketTypeId { get; set; }
    public int Quantity { get; set; }
}

/// <summary>
/// 价格计算请求DTO
/// </summary>
public class CalculatePriceRequestDto
{
    public int VisitorId { get; set; }
    public List<ReservationItemRequestDto> Items { get; set; } = [];
    public string? PromotionCode { get; set; }
}

/// <summary>
/// 价格计算结果DTO
/// </summary>
public class ReservationPriceCalculationDto
{
    public decimal SubtotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public List<ReservationItemPriceDto> Items { get; set; } = [];
    public PromotionDto? AppliedPromotion { get; set; }
}

/// <summary>
/// 预订项目价格明细DTO
/// </summary>
public class ReservationItemPriceDto
{
    public int TicketTypeId { get; set; }
    public string TicketTypeName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
}

/// <summary>
/// 促销DTO
/// </summary>
public class PromotionDto
{
    public int PromotionId { get; set; }
    public string PromotionName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public PromotionType PromotionType { get; set; }
    public string? PromoCode { get; set; }
    public bool IsActive { get; set; }
}

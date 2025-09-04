using DbApp.Domain.Enums.TicketingSystem;
using MediatR;

namespace DbApp.Application.TicketingSystem.Tickets;

/// <summary>
/// Search ticket sale records with filtering and pagination.
/// </summary>
public record SearchTicketSaleQuery(
    string? Keyword = null, // Search in ticket code, visitor name, email, ticket type name, description.
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    int? TicketTypeId = null,
    int? PromotionId = null,
    PaymentStatus? PaymentStatus = null,
    string? SortBy = "SalesDate", // Sort by "SalesDate", "BasePrice", "SerialNumber", "VisitorName".
    bool Descending = true,
    int Page = 1,
    int PageSize = 20
) : IRequest<TicketSaleResult>;

/// <summary>
/// Get overall ticket sale statistics with filtering.
/// </summary>
public record GetTicketSaleStatsQuery(
    string? Keyword = null, // Search in ticket code, visitor name, email, ticket type name, description.
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    int? TicketTypeId = null,
    int? PromotionId = null,
    PaymentStatus? PaymentStatus = null
) : IRequest<TicketSaleStatsDto>;

/// <summary>
/// Get ticket sale statistics grouped by different parameters with filtering.
/// </summary>
public record GetGroupedTicketSaleStatsQuery(
    string? Keyword = null, // Search in ticket code, visitor name, email, ticket type name, description.
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    int? TicketTypeId = null,
    int? PromotionId = null,
    PaymentStatus? PaymentStatus = null,
    string GroupBy = "TicketType", // Group by "TicketType", "Promotion", "Date", "PaymentStatus".
    string? SortBy = "Revenue", // Sort by "Revenue", "TicketsSold", "RefundRate", "GroupName".
    bool Descending = true
) : IRequest<List<GroupedTicketSaleStatsDto>>;

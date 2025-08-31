using DbApp.Domain.Entities.TicketingSystem;
using DbApp.Domain.Enums.TicketingSystem;
using DbApp.Domain.Interfaces.TicketingSystem;
using DbApp.Domain.Specifications.TicketingSystem;
using DbApp.Domain.Statistics.TicketingSystem;
using Microsoft.EntityFrameworkCore;

namespace DbApp.Infrastructure.Repositories.TicketingSystem;

public class TicketRepository(ApplicationDbContext dbContext) : ITicketRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<List<Ticket>> SearchAsync(TicketSaleSearchSpec spec)
    {
        var query = _dbContext.Tickets
            .Include(t => t.TicketType)
            .Include(t => t.Visitor.User)
            .Include(t => t.ReservationItem.Reservation.Promotion)
            .AsQueryable();

        query = ApplyFilters(query, spec.Keyword, spec.StartDate, spec.EndDate,
            spec.TicketTypeId, spec.PromotionId, spec.PaymentStatus);

        query = ApplySorting(query, spec.SortBy, spec.Descending);

        return await query
            .Skip((spec.Page - 1) * spec.PageSize)
            .Take(spec.PageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync(TicketSaleCountSpec spec)
    {
        var query = _dbContext.Tickets.AsQueryable();
        query = ApplyFilters(query, spec.Keyword, spec.StartDate, spec.EndDate,
            spec.TicketTypeId, spec.PromotionId, spec.PaymentStatus);
        return await query.CountAsync();
    }

    public async Task<TicketSaleStats> GetStatsAsync(TicketSaleStatsSpec spec)
    {
        var query = _dbContext.Tickets
            .Include(t => t.ReservationItem)
            .AsQueryable();

        query = ApplyFilters(query, spec.Keyword, spec.StartDate, spec.EndDate,
            spec.TicketTypeId, spec.PromotionId, spec.PaymentStatus);

        var totalTicketsSold = await query.CountAsync();
        var totalRevenue = await query
            .Select(t => t.ReservationItem)
            .Distinct()
            .SumAsync(ri => ri.TotalAmount);
        var totalRefunded = await _dbContext.RefundRecords
            .Where(rr => query.Any(t => t.TicketId == rr.TicketId))
            .SumAsync(rr => rr.RefundAmount);
        var totalRefundedTickets = await _dbContext.RefundRecords
            .Where(rr => query.Any(t => t.TicketId == rr.TicketId))
            .CountAsync();
        var firstSale = await query
            .OrderBy(t => t.ReservationItem.Reservation.CreatedAt)
            .Select(t => (DateTime?)t.ReservationItem.Reservation.CreatedAt)
            .FirstOrDefaultAsync();
        var lastSale = await query
            .OrderByDescending(t => t.ReservationItem.Reservation.CreatedAt)
            .Select(t => (DateTime?)t.ReservationItem.Reservation.CreatedAt)
            .FirstOrDefaultAsync();

        return new TicketSaleStats
        {
            TotalTicketsSold = totalTicketsSold,
            TotalRevenue = totalRevenue,
            TotalRefunded = totalRefunded,
            NetRevenue = totalRevenue - totalRefunded,
            AverageTicketPrice = totalTicketsSold > 0 ? totalRevenue / totalTicketsSold : 0,
            TotalRefundedTickets = totalRefundedTickets,
            RefundRate = totalTicketsSold > 0 ? (decimal)totalRefundedTickets / totalTicketsSold : 0,
            FirstSale = firstSale,
            LastSale = lastSale
        };
    }

    public async Task<List<GroupedTicketSaleStats>> GetGroupedStatsAsync(TicketSaleGroupedStatsSpec spec)
    {
        var query = _dbContext.Tickets
            .Include(t => t.TicketType)
            .Include(t => t.ReservationItem.Reservation.Promotion)
            .AsQueryable();

        query = ApplyFilters(query, spec.Keyword, spec.StartDate, spec.EndDate,
            spec.TicketTypeId, spec.PromotionId, spec.PaymentStatus);

        var groupedQuery = spec.GroupBy switch
        {
            "TicketType" => query.GroupBy(t => new
            {
                Key = t.TicketTypeId.ToString(),
                Name = t.TicketType.TypeName
            }),
            "Promotion" => query.GroupBy(t => new
            {
                Key = t.ReservationItem.Reservation.PromotionId.ToString() ?? "None",
                Name = t.ReservationItem.Reservation.Promotion != null
                    ? t.ReservationItem.Reservation.Promotion.PromotionName
                    : "No Promotion"
            }),
            "PaymentStatus" => query.GroupBy(t => new
            {
                Key = t.ReservationItem.Reservation.PaymentStatus.ToString(),
                Name = t.ReservationItem.Reservation.PaymentStatus.ToString()
            }),
            "Date" => query.GroupBy(t => new
            {
                Key = t.ReservationItem.Reservation.CreatedAt.Date.ToString("yyyy-MM-dd"),
                Name = t.ReservationItem.Reservation.CreatedAt.Date.ToString("yyyy-MM-dd")
            }),
            _ => query.GroupBy(t => new
            {
                Key = t.TicketTypeId.ToString(),
                Name = t.TicketType.TypeName
            })
        };

        var results = await groupedQuery.Select(g => new GroupedTicketSaleStats
        {
            GroupKey = g.Key.Key,
            GroupName = g.Key.Name,
            TicketsSold = g.Count(),
            Revenue = g
                .Select(t => t.ReservationItem).Distinct().Sum(ri => ri.TotalAmount),
            AveragePrice = g
                .Select(t => t.ReservationItem).Distinct().Sum(ri => ri.TotalAmount) / g.Count(),
            RefundedTickets = _dbContext.RefundRecords
                .Count(rr => g.Any(t => t.TicketId == rr.TicketId)),
            RefundAmount = _dbContext.RefundRecords
                .Where(rr => g.Any(t => t.TicketId == rr.TicketId)).Sum(rr => rr.RefundAmount),
            FirstSale = g.Min(t => (DateTime?)t.ReservationItem.Reservation.CreatedAt),
            LastSale = g.Max(t => (DateTime?)t.ReservationItem.Reservation.CreatedAt)
        }).ToListAsync();

        // Calculate RefundRate.
        foreach (var result in results)
        {
            result.RefundRate = result.TicketsSold > 0 ? (decimal)result.RefundedTickets / result.TicketsSold : 0;
        }

        // Apply sorting to grouped results.
        results = ApplyGroupedSorting(results, spec.SortBy, spec.Descending);

        return results;
    }

    private static IQueryable<Ticket> ApplyFilters(
        IQueryable<Ticket> query,
        string? keyword,
        DateTime? startDate,
        DateTime? endDate,
        int? ticketTypeId,
        int? promotionId,
        PaymentStatus? paymentStatus)
    {
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(t =>
                t.SerialNumber.Contains(keyword) ||
                (t.Visitor != null && t.Visitor.User != null && (
                    t.Visitor.User.Username.Contains(keyword) ||
                    t.Visitor.User.Email.Contains(keyword)
                )) ||
                t.TicketType.TypeName.Contains(keyword)
            );
        }

        if (startDate.HasValue)
            query = query.Where(t => t.ReservationItem.Reservation.CreatedAt >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(t => t.ReservationItem.Reservation.CreatedAt <= endDate.Value);

        if (ticketTypeId.HasValue)
            query = query.Where(t => t.TicketTypeId == ticketTypeId.Value);

        if (promotionId.HasValue)
            query = query.Where(t => t.ReservationItem.Reservation.PromotionId == promotionId.Value);

        if (paymentStatus.HasValue)
            query = query.Where(t => t.ReservationItem.Reservation.PaymentStatus == paymentStatus.Value);

        return query;
    }

    private static IQueryable<Ticket> ApplySorting(
        IQueryable<Ticket> query,
        string? sortBy,
        bool descending)
    {
        return sortBy switch
        {
            "BasePrice" => descending
                ? query.OrderByDescending(t => t.TicketType.BasePrice)
                : query.OrderBy(t => t.TicketType.BasePrice),
            "SerialNumber" => descending
                ? query.OrderByDescending(t => t.SerialNumber)
                : query.OrderBy(t => t.SerialNumber),
            "VisitorName" => descending
                ? query.OrderByDescending(t => t.Visitor.User.Username)
                : query.OrderBy(t => t.Visitor.User.Username),
            _ => descending
                ? query.OrderByDescending(t => t.ReservationItem.Reservation.CreatedAt)
                : query.OrderBy(t => t.ReservationItem.Reservation.CreatedAt)
        };
    }

    private static List<GroupedTicketSaleStats> ApplyGroupedSorting(
        List<GroupedTicketSaleStats> results,
        string? sortBy,
        bool descending)
    {
        return sortBy switch
        {
            "TicketsSold" => descending
                ? [.. results.OrderByDescending(r => r.TicketsSold)]
                : [.. results.OrderBy(r => r.TicketsSold)],
            "RefundRate" => descending
                ? [.. results.OrderByDescending(r => r.RefundRate)]
                : [.. results.OrderBy(r => r.RefundRate)],
            "GroupName" => descending
                ? [.. results.OrderByDescending(r => r.GroupName)]
                : [.. results.OrderBy(r => r.GroupName)],
            _ => descending
                ? [.. results.OrderByDescending(r => r.Revenue)]
                : [.. results.OrderBy(r => r.Revenue)]
        };
    }
}

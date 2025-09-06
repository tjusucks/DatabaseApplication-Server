using DbApp.Domain.Entities.TicketingSystem;
using DbApp.Domain.Enums.TicketingSystem;
using DbApp.Domain.Interfaces.TicketingSystem;
using DbApp.Domain.Specifications.TicketingSystem;
using DbApp.Domain.Statistics.TicketingSystem;
using Microsoft.EntityFrameworkCore;

namespace DbApp.Infrastructure.Repositories.TicketingSystem;

public class ReservationRepository(ApplicationDbContext dbContext) : IReservationRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<List<Reservation>> SearchByVisitorAsync(ReservationSearchByVisitorSpec spec)
    {
        var query = _dbContext.Reservations
            .Include(r => r.Visitor.User)
            .Include(r => r.Promotion)
            .Include(r => r.ReservationItems)
                .ThenInclude(ri => ri.TicketType)
            .AsQueryable();

        query = ApplyFilters(query, spec.VisitorId, spec.Keyword, spec.StartDate, spec.EndDate,
            spec.PaymentStatus, spec.Status, spec.MinAmount, spec.MaxAmount, spec.PromotionId);

        query = ApplySorting(query, spec.SortBy, spec.Descending);

        return await query
            .Skip((spec.Page - 1) * spec.PageSize)
            .Take(spec.PageSize)
            .ToListAsync();
    }

    public async Task<int> CountByVisitorAsync(ReservationCountByVisitorSpec spec)
    {
        var query = _dbContext.Reservations.AsQueryable();
        query = ApplyFilters(query, spec.VisitorId, spec.Keyword, spec.StartDate, spec.EndDate,
            spec.PaymentStatus, spec.Status, spec.MinAmount, spec.MaxAmount, spec.PromotionId);
        return await query.CountAsync();
    }

    public async Task<List<Reservation>> SearchAsync(ReservationSearchSpec spec)
    {
        var query = _dbContext.Reservations
            .Include(r => r.Visitor.User)
            .Include(r => r.Promotion)
            .Include(r => r.ReservationItems)
                .ThenInclude(ri => ri.TicketType)
            .AsQueryable();

        query = ApplyFilters(query, spec.VisitorId, spec.Keyword, spec.StartDate, spec.EndDate,
            spec.PaymentStatus, spec.Status, spec.MinAmount, spec.MaxAmount, spec.PromotionId);

        query = ApplySorting(query, spec.SortBy, spec.Descending);

        return await query
            .Skip((spec.Page - 1) * spec.PageSize)
            .Take(spec.PageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync(ReservationCountSpec spec)
    {
        var query = _dbContext.Reservations.AsQueryable();
        query = ApplyFilters(query, spec.VisitorId, spec.Keyword, spec.StartDate, spec.EndDate,
            spec.PaymentStatus, spec.Status, spec.MinAmount, spec.MaxAmount, spec.PromotionId);
        return await query.CountAsync();
    }

    public async Task<ReservationStats> GetStatsByVisitorAsync(ReservationStatsByVisitorSpec spec)
    {
        var query = _dbContext.Reservations.AsQueryable();
        query = ApplyFilters(query, spec.VisitorId, null, spec.StartDate, spec.EndDate, null, null, null, null, null);

        var totalReservations = await query.CountAsync();
        var totalSpent = await query.SumAsync(r => r.TotalAmount);
        var totalRefunded = await _dbContext.RefundRecords
            .Where(rr => query.Any(r => r.ReservationItems.Any(ri => ri.Tickets.Any(t => t.TicketId == rr.TicketId))))
            .SumAsync(rr => rr.RefundAmount);
        var totalTickets = await query.SelectMany(r => r.ReservationItems).SumAsync(i => i.Quantity);
        var firstReservation = await query.OrderBy(r => r.ReservationTime)
            .Select(r => (DateTime?)r.ReservationTime).FirstOrDefaultAsync();
        var lastReservation = await query.OrderByDescending(r => r.ReservationTime)
            .Select(r => (DateTime?)r.ReservationTime).FirstOrDefaultAsync();

        return new ReservationStats
        {
            TotalReservations = totalReservations,
            TotalSpent = totalSpent,
            TotalRefunded = totalRefunded,
            TotalTickets = totalTickets,
            FirstReservation = firstReservation,
            LastReservation = lastReservation
        };
    }

    private static IQueryable<Reservation> ApplyFilters(
        IQueryable<Reservation> query,
        int? visitorId,
        string? keyword,
        DateTime? startDate,
        DateTime? endDate,
        PaymentStatus? paymentStatus,
        ReservationStatus? status,
        decimal? minAmount,
        decimal? maxAmount,
        int? promotionId)
    {
        if (visitorId.HasValue)
            query = query.Where(r => r.VisitorId == visitorId.Value);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(r =>
                (r.Visitor != null && r.Visitor.User != null && (
                    r.Visitor.User.Username.Contains(keyword) ||
                    (r.Visitor.User.Email != null && r.Visitor.User.Email.Contains(keyword)) ||
                    (r.Visitor.User.PhoneNumber != null && r.Visitor.User.PhoneNumber.Contains(keyword))
                )) ||
                (r.Promotion != null && r.Promotion.PromotionName.Contains(keyword)) ||
                r.ReservationItems.Any(ri => ri.TicketType.TypeName.Contains(keyword))
            );
        }

        if (startDate.HasValue)
            query = query.Where(r => r.ReservationTime >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(r => r.ReservationTime <= endDate.Value);

        if (paymentStatus.HasValue)
            query = query.Where(r => r.PaymentStatus == paymentStatus.Value);

        if (status.HasValue)
            query = query.Where(r => r.Status == status.Value);

        if (minAmount.HasValue)
            query = query.Where(r => r.TotalAmount >= minAmount.Value);

        if (maxAmount.HasValue)
            query = query.Where(r => r.TotalAmount <= maxAmount.Value);

        if (promotionId.HasValue)
            query = query.Where(r => r.PromotionId == promotionId.Value);

        return query;
    }

    private static IQueryable<Reservation> ApplySorting(
        IQueryable<Reservation> query,
        string? sortBy,
        bool descending)
    {
        return sortBy switch
        {
            "TotalAmount" => descending
                ? query.OrderByDescending(r => r.TotalAmount)
                : query.OrderBy(r => r.TotalAmount),
            "VisitDate" => descending
                ? query.OrderByDescending(r => r.VisitDate)
                : query.OrderBy(r => r.VisitDate),
            _ => descending
                ? query.OrderByDescending(r => r.ReservationTime)
                : query.OrderBy(r => r.ReservationTime)
        };
    }
}

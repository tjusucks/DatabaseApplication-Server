using DbApp.Domain.Entities.TicketingSystem;
using DbApp.Domain.Enums.TicketingSystem;
using DbApp.Domain.Interfaces.TicketingSystem;
using DbApp.Domain.Statistics.TicketingSystem;
using Microsoft.EntityFrameworkCore;

namespace DbApp.Infrastructure.Repositories.TicketingSystem;

public class ReservationRepository(ApplicationDbContext dbContext) : IReservationRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<List<Reservation>> SearchByVisitorAsync(
        int visitorId,
        string? keyword = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        PaymentStatus? paymentStatus = null,
        ReservationStatus? status = null,
        decimal? minAmount = null,
        decimal? maxAmount = null,
        int? promotionId = null,
        string? sortBy = "ReservationTime",
        bool descending = true,
        int page = 1,
        int pageSize = 20)
    {
        var query = _dbContext.Reservations
            .Include(r => r.Visitor.User)
            .Include(r => r.Promotion)
            .Include(r => r.ReservationItems)
            .AsQueryable();

        query = ApplyFilters(query, visitorId, keyword, startDate, endDate, paymentStatus, status, minAmount, maxAmount, promotionId);

        query = ApplySorting(query, sortBy, descending);

        return await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountByVisitorAsync(
        int visitorId,
        string? keyword = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        PaymentStatus? paymentStatus = null,
        ReservationStatus? status = null,
        decimal? minAmount = null,
        decimal? maxAmount = null,
        int? promotionId = null)
    {
        var query = _dbContext.Reservations.AsQueryable();
        query = ApplyFilters(query, visitorId, keyword, startDate, endDate, paymentStatus, status, minAmount, maxAmount, promotionId);
        return await query.CountAsync();
    }

    public async Task<List<Reservation>> SearchAsync(
        int? visitorId = null,
        string? keyword = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        PaymentStatus? paymentStatus = null,
        ReservationStatus? status = null,
        decimal? minAmount = null,
        decimal? maxAmount = null,
        int? promotionId = null,
        string? sortBy = "ReservationTime",
        bool descending = true,
        int page = 1,
        int pageSize = 20)
    {
        var query = _dbContext.Reservations
            .Include(r => r.Visitor.User)
            .Include(r => r.Promotion)
            .Include(r => r.ReservationItems)
            .AsQueryable();

        query = ApplyFilters(query, visitorId, keyword, startDate, endDate, paymentStatus, status, minAmount, maxAmount, promotionId);

        query = ApplySorting(query, sortBy, descending);

        return await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync(
        int? visitorId = null,
        string? keyword = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        PaymentStatus? paymentStatus = null,
        ReservationStatus? status = null,
        decimal? minAmount = null,
        decimal? maxAmount = null,
        int? promotionId = null)
    {
        var query = _dbContext.Reservations.AsQueryable();
        query = ApplyFilters(query, visitorId, keyword, startDate, endDate, paymentStatus, status, minAmount, maxAmount, promotionId);
        return await query.CountAsync();
    }

    public async Task<ReservationRecordStats> GetStatsByVisitorAsync(
        int visitorId,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        var query = _dbContext.Reservations.AsQueryable();
        query = ApplyFilters(query, visitorId, null, startDate, endDate, null, null, null, null, null);

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

        return new ReservationRecordStats
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
                    r.Visitor.User.Email.Contains(keyword) ||
                    (r.Visitor.User.PhoneNumber != null && r.Visitor.User.PhoneNumber.Contains(keyword))
                )) ||
                (r.Promotion != null && r.Promotion.PromotionName.Contains(keyword))
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

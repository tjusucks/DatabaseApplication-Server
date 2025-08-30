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
        DateTime? startDate,
        DateTime? endDate,
        PaymentStatus? paymentStatus,
        ReservationStatus? status,
        string? sortBy,
        bool descending,
        int page,
        int pageSize)
    {
        var query = _dbContext.Reservations
            .Include(r => r.Visitor)
            .Include(r => r.Promotion)
            .Include(r => r.ReservationItems)
            .Where(r => r.VisitorId == visitorId);

        if (startDate.HasValue)
            query = query.Where(r => r.ReservationTime >= startDate.Value);
        if (endDate.HasValue)
            query = query.Where(r => r.ReservationTime <= endDate.Value);
        if (paymentStatus.HasValue)
            query = query.Where(r => r.PaymentStatus == paymentStatus.Value);
        if (status.HasValue)
            query = query.Where(r => r.Status == status.Value);

        query = sortBy switch
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

        return await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountByVisitorAsync(
        int visitorId,
        DateTime? startDate,
        DateTime? endDate,
        PaymentStatus? paymentStatus,
        ReservationStatus? status)
    {
        var query = _dbContext.Reservations.Where(r => r.VisitorId == visitorId);

        if (startDate.HasValue)
            query = query.Where(r => r.ReservationTime >= startDate.Value);
        if (endDate.HasValue)
            query = query.Where(r => r.ReservationTime <= endDate.Value);
        if (paymentStatus.HasValue)
            query = query.Where(r => r.PaymentStatus == paymentStatus.Value);
        if (status.HasValue)
            query = query.Where(r => r.Status == status.Value);

        return await query.CountAsync();
    }

    public async Task<List<Reservation>> SearchAsync(
        string? keyword,
        DateTime? startDate,
        DateTime? endDate,
        PaymentStatus? paymentStatus,
        ReservationStatus? status,
        decimal? minAmount,
        decimal? maxAmount,
        int? promotionId,
        string? sortBy,
        bool descending,
        int page,
        int pageSize)
    {
        var query = _dbContext.Reservations
            .Include(r => r.Visitor)
            .Include(r => r.Promotion)
            .Include(r => r.ReservationItems)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(r =>
                r.Visitor != null &&
                r.Visitor.User != null &&
                (
                    r.Visitor.User.Username.Contains(keyword) ||
                    r.Visitor.User.Email.Contains(keyword) ||
                    (r.Visitor.User.PhoneNumber != null && r.Visitor.User.PhoneNumber.Contains(keyword))
                )
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

        query = sortBy switch
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

        return await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync(
        string? keyword,
        DateTime? startDate,
        DateTime? endDate,
        PaymentStatus? paymentStatus,
        ReservationStatus? status,
        decimal? minAmount,
        decimal? maxAmount,
        int? promotionId)
    {
        var query = _dbContext.Reservations.AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(r =>
                r.Visitor != null && (
                    r.Visitor.User.Username.Contains(keyword) ||
                    r.Visitor.User.Email.Contains(keyword) ||
                    (r.Visitor.User.PhoneNumber != null && r.Visitor.User.PhoneNumber.Contains(keyword))
                )
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

        return await query.CountAsync();
    }

    public async Task<ReservationRecordStats> GetStatsByVisitorAsync(
        int visitorId,
        DateTime? startDate,
        DateTime? endDate)
    {
        var query = _dbContext.Reservations.Where(r => r.VisitorId == visitorId);

        if (startDate.HasValue)
            query = query.Where(r => r.ReservationTime >= startDate.Value);
        if (endDate.HasValue)
            query = query.Where(r => r.ReservationTime <= endDate.Value);

        var totalReservations = await query.CountAsync();
        var totalSpent = await query.SumAsync(r => r.TotalAmount);
        var totalRefunded = await _dbContext.RefundRecords
            .Where(rr => rr.Ticket.ReservationItem.Reservation.VisitorId == visitorId)
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
}

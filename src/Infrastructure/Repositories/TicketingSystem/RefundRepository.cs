using Microsoft.EntityFrameworkCore;
using DbApp.Domain.Entities.TicketingSystem;
using DbApp.Domain.Interfaces.TicketingSystem;
using DbApp.Domain.Enums.TicketingSystem;
using DbApp.Infrastructure;

namespace DbApp.Infrastructure.Repositories.TicketingSystem;

public class RefundRepository(ApplicationDbContext dbContext) : IRefundRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<RefundRecord> AddAsync(RefundRecord refundRecord)
    {
        refundRecord.CreatedAt = DateTime.UtcNow;
        refundRecord.UpdatedAt = DateTime.UtcNow;

        _dbContext.RefundRecords.Add(refundRecord);
        await _dbContext.SaveChangesAsync();
        return refundRecord;
    }

    public async Task<RefundRecord> UpdateAsync(RefundRecord refundRecord)
    {
        refundRecord.UpdatedAt = DateTime.UtcNow;

        _dbContext.RefundRecords.Update(refundRecord);
        await _dbContext.SaveChangesAsync();
        return refundRecord;
    }

    public async Task<RefundRecord?> GetByIdAsync(int refundId)
    {
        return await _dbContext.RefundRecords
            .Include(rr => rr.Ticket)
                .ThenInclude(t => t.TicketType)
            .Include(rr => rr.Ticket)
                .ThenInclude(t => t.ReservationItem)
                .ThenInclude(ri => ri.Reservation)
            .Include(rr => rr.Visitor)
                .ThenInclude(v => v.User)
            .Include(rr => rr.Processor)
            .FirstOrDefaultAsync(rr => rr.RefundId == refundId);
    }

    public async Task<RefundRecord?> GetByTicketIdAsync(int ticketId)
    {
        return await _dbContext.RefundRecords
            .Include(rr => rr.Ticket)
                .ThenInclude(t => t.TicketType)
            .Include(rr => rr.Ticket)
                .ThenInclude(t => t.ReservationItem)
                .ThenInclude(ri => ri.Reservation)
            .Include(rr => rr.Visitor)
                .ThenInclude(v => v.User)
            .Include(rr => rr.Processor)
            .FirstOrDefaultAsync(rr => rr.TicketId == ticketId);
    }

    public async Task<bool> DeleteAsync(int refundId)
    {
        var refundRecord = await _dbContext.RefundRecords.FindAsync(refundId);
        if (refundRecord == null) return false;

        _dbContext.RefundRecords.Remove(refundRecord);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<List<RefundRecord>> SearchAsync(
        string? keyword = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int? visitorId = null,
        int? ticketTypeId = null,
        RefundStatus? status = null,
        decimal? minAmount = null,
        decimal? maxAmount = null,
        string? sortBy = "RefundTime",
        bool descending = true,
        int page = 1,
        int pageSize = 20)
    {
        var query = _dbContext.RefundRecords
            .Include(rr => rr.Ticket)
                .ThenInclude(t => t.TicketType)
            .Include(rr => rr.Ticket)
                .ThenInclude(t => t.ReservationItem)
                .ThenInclude(ri => ri.Reservation)
            .Include(rr => rr.Visitor)
                .ThenInclude(v => v.User)
            .Include(rr => rr.Processor)
            .AsQueryable();

        query = ApplyFilters(query, keyword, startDate, endDate, visitorId, 
            ticketTypeId, status, minAmount, maxAmount);

        query = ApplySorting(query, sortBy, descending);

        return await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync(
        string? keyword = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int? visitorId = null,
        int? ticketTypeId = null,
        RefundStatus? status = null,
        decimal? minAmount = null,
        decimal? maxAmount = null)
    {
        var query = _dbContext.RefundRecords
            .Include(rr => rr.Ticket)
                .ThenInclude(t => t.TicketType)
            .Include(rr => rr.Visitor)
                .ThenInclude(v => v.User)
            .AsQueryable();

        query = ApplyFilters(query, keyword, startDate, endDate, visitorId, 
            ticketTypeId, status, minAmount, maxAmount);

        return await query.CountAsync();
    }

    public async Task<RefundStatistics> GetStatisticsAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        int? ticketTypeId = null,
        RefundStatus? status = null)
    {
        var query = _dbContext.RefundRecords
            .Include(rr => rr.Ticket)
                .ThenInclude(t => t.TicketType)
            .AsQueryable();

        query = ApplyFilters(query, null, startDate, endDate, null, 
            ticketTypeId, status, null, null);

        var stats = new RefundStatistics
        {
            TotalRefunds = await query.CountAsync(),
            TotalRefundAmount = await query.SumAsync(rr => rr.RefundAmount),
            PendingRefunds = await query.CountAsync(rr => rr.RefundStatus == RefundStatus.Pending),
            ApprovedRefunds = await query.CountAsync(rr => rr.RefundStatus == RefundStatus.Approved),
            RejectedRefunds = await query.CountAsync(rr => rr.RefundStatus == RefundStatus.Rejected),
            CompletedRefunds = await query.CountAsync(rr => rr.RefundStatus == RefundStatus.Completed),
            FirstRefund = await query.MinAsync(rr => (DateTime?)rr.RefundTime),
            LastRefund = await query.MaxAsync(rr => (DateTime?)rr.RefundTime)
        };

        if (stats.TotalRefunds > 0)
        {
            stats.AverageRefundAmount = stats.TotalRefundAmount / stats.TotalRefunds;
        }

        return stats;
    }

    public async Task<bool> IsTicketRefundedAsync(int ticketId)
    {
        return await _dbContext.RefundRecords
            .AnyAsync(rr => rr.TicketId == ticketId);
    }

    public async Task<List<RefundRecord>> GetByVisitorIdAsync(int visitorId, int page = 1, int pageSize = 20)
    {
        return await _dbContext.RefundRecords
            .Include(rr => rr.Ticket)
                .ThenInclude(t => t.TicketType)
            .Include(rr => rr.Processor)
            .Where(rr => rr.VisitorId == visitorId)
            .OrderByDescending(rr => rr.RefundTime)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    private static IQueryable<RefundRecord> ApplyFilters(
        IQueryable<RefundRecord> query,
        string? keyword,
        DateTime? startDate,
        DateTime? endDate,
        int? visitorId,
        int? ticketTypeId,
        RefundStatus? status,
        decimal? minAmount,
        decimal? maxAmount)
    {
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(rr =>
                rr.Ticket.SerialNumber.Contains(keyword) ||
                (rr.Visitor.User != null && (
                    rr.Visitor.User.Username.Contains(keyword) ||
                    rr.Visitor.User.Email.Contains(keyword) ||
                    rr.Visitor.User.DisplayName.Contains(keyword)
                )) ||
                rr.Ticket.TicketType.TypeName.Contains(keyword) ||
                (rr.RefundReason != null && rr.RefundReason.Contains(keyword))
            );
        }

        if (startDate.HasValue)
            query = query.Where(rr => rr.RefundTime >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(rr => rr.RefundTime <= endDate.Value);

        if (visitorId.HasValue)
            query = query.Where(rr => rr.VisitorId == visitorId.Value);

        if (ticketTypeId.HasValue)
            query = query.Where(rr => rr.Ticket.TicketTypeId == ticketTypeId.Value);

        if (status.HasValue)
            query = query.Where(rr => rr.RefundStatus == status.Value);

        if (minAmount.HasValue)
            query = query.Where(rr => rr.RefundAmount >= minAmount.Value);

        if (maxAmount.HasValue)
            query = query.Where(rr => rr.RefundAmount <= maxAmount.Value);

        return query;
    }

    private static IQueryable<RefundRecord> ApplySorting(
        IQueryable<RefundRecord> query,
        string? sortBy,
        bool descending)
    {
        return sortBy?.ToLower() switch
        {
            "refundamount" => descending
                ? query.OrderByDescending(rr => rr.RefundAmount)
                : query.OrderBy(rr => rr.RefundAmount),
            "visitorname" => descending
                ? query.OrderByDescending(rr => rr.Visitor.User != null ? rr.Visitor.User.DisplayName : "")
                : query.OrderBy(rr => rr.Visitor.User != null ? rr.Visitor.User.DisplayName : ""),
            "tickettype" => descending
                ? query.OrderByDescending(rr => rr.Ticket.TicketType.TypeName)
                : query.OrderBy(rr => rr.Ticket.TicketType.TypeName),
            "status" => descending
                ? query.OrderByDescending(rr => rr.RefundStatus)
                : query.OrderBy(rr => rr.RefundStatus),
            _ => descending
                ? query.OrderByDescending(rr => rr.RefundTime)
                : query.OrderBy(rr => rr.RefundTime)
        };
    }
}

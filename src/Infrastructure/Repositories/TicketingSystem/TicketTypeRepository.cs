using DbApp.Domain.Entities.TicketingSystem;
using DbApp.Domain.Interfaces.TicketingSystem;
using Microsoft.EntityFrameworkCore;

namespace DbApp.Infrastructure.Repositories.TicketingSystem;

public class TicketTypeRepository(ApplicationDbContext dbContext) : ITicketTypeRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<TicketType?> GetByIdAsync(int ticketTypeId)
    {
        return await _dbContext.TicketTypes
            .FirstOrDefaultAsync(tt => tt.TicketTypeId == ticketTypeId);
    }

    public async Task<List<TicketType>> GetActiveTicketTypesAsync()
    {
        return await _dbContext.TicketTypes
            .Where(tt => tt.BasePrice > 0) // 简单的活跃标准
            .OrderBy(tt => tt.TypeName)
            .ToListAsync();
    }

    public async Task<int> GetSoldCountAsync(int ticketTypeId, DateTime visitDate)
    {
        var startOfDay = visitDate.Date;
        var endOfDay = startOfDay.AddDays(1);

        return await _dbContext.ReservationItems
            .Where(ri => ri.TicketTypeId == ticketTypeId &&
                        ri.Reservation.VisitDate >= startOfDay &&
                        ri.Reservation.VisitDate < endOfDay &&
                        ri.Reservation.Status != DbApp.Domain.Enums.TicketingSystem.ReservationStatus.Cancelled)
            .SumAsync(ri => ri.Quantity);
    }

    public async Task<bool> HasSufficientStockAsync(int ticketTypeId, DateTime visitDate, int requestedQuantity)
    {
        var ticketType = await GetByIdAsync(ticketTypeId);
        if (ticketType?.MaxSaleLimit == null) return true; // 无限制

        var soldCount = await GetSoldCountAsync(ticketTypeId, visitDate);
        return soldCount + requestedQuantity <= ticketType.MaxSaleLimit.Value;
    }
}

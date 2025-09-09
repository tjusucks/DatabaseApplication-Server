using MediatR;
using Microsoft.EntityFrameworkCore;
using DbApp.Infrastructure;

namespace DbApp.Application.TicketingSystem.Reservations;

public class TicketTypeQueryHandler(ApplicationDbContext context) : 
    IRequestHandler<GetAvailableTicketTypesQuery, List<TicketTypeDto>>,
    IRequestHandler<GetTicketTypeByIdQuery, TicketTypeDto?>
{
    private readonly ApplicationDbContext _context = context;

    /// <summary>
    /// 获取所有可用票种
    /// </summary>
    public async Task<List<TicketTypeDto>> Handle(GetAvailableTicketTypesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.TicketTypes.AsQueryable();

        var ticketTypes = await query
            .Select(tt => new TicketTypeDto
            {
                TicketTypeId = tt.TicketTypeId,
                TypeName = tt.TypeName,
                Description = tt.Description,
                BasePrice = tt.BasePrice,
                RulesText = tt.RulesText,
                MaxSaleLimit = tt.MaxSaleLimit,
                ApplicableCrowd = tt.ApplicableCrowd,
                IsAvailable = true, 
                RemainingQuantity = tt.MaxSaleLimit ?? int.MaxValue
            })
            .ToListAsync(cancellationToken);

        return ticketTypes;
    }

    /// <summary>
    /// 根据ID获取特定票种
    /// </summary>
    public async Task<TicketTypeDto?> Handle(GetTicketTypeByIdQuery request, CancellationToken cancellationToken)
    {
        var ticketType = await _context.TicketTypes
            .Where(tt => tt.TicketTypeId == request.TicketTypeId)
            .Select(tt => new TicketTypeDto
            {
                TicketTypeId = tt.TicketTypeId,
                TypeName = tt.TypeName,
                Description = tt.Description,
                BasePrice = tt.BasePrice,
                RulesText = tt.RulesText,
                MaxSaleLimit = tt.MaxSaleLimit,
                ApplicableCrowd = tt.ApplicableCrowd,
                IsAvailable = true,
                RemainingQuantity = tt.MaxSaleLimit ?? int.MaxValue
            })
            .FirstOrDefaultAsync(cancellationToken);

        return ticketType;
    }
}

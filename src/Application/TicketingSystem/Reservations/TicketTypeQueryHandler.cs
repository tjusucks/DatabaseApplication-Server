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

        // 如果指定了日期，可以根据日期过滤（例如季节性票种）
        if (request.ForDate.HasValue)
        {
            // 基于日期的过滤逻辑待实现
        }

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
                IsAvailable = true, // 简化处理，后续可添加库存检查逻辑
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

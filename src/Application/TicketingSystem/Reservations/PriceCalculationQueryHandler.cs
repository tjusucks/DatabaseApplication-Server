using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DbApp.Infrastructure;

namespace DbApp.Application.TicketingSystem.Reservations;

public class PriceCalculationQueryHandler(ApplicationDbContext context, ILogger<PriceCalculationQueryHandler> logger) 
    : IRequestHandler<CalculateReservationPriceQuery, ReservationPriceCalculationDto>
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<PriceCalculationQueryHandler> _logger = logger;

    public async Task<ReservationPriceCalculationDto> Handle(CalculateReservationPriceQuery request, CancellationToken cancellationToken)
    {
        var result = new ReservationPriceCalculationDto();

        try
        {
            // 1. 获取所有票种信息
            var ticketTypeIds = request.Items.Select(item => item.TicketTypeId).ToList();
            var ticketTypes = await _context.TicketTypes
                .Where(tt => ticketTypeIds.Contains(tt.TicketTypeId))
                .ToListAsync(cancellationToken);

            // 2. 计算每个项目的价格
            foreach (var item in request.Items)
            {
                var ticketType = ticketTypes.FirstOrDefault(tt => tt.TicketTypeId == item.TicketTypeId);
                if (ticketType == null)
                {
                    throw new InvalidOperationException($"票种ID {item.TicketTypeId} 不存在");
                }

                // 基础价格计算
                var unitPrice = ticketType.BasePrice;
                var subtotal = unitPrice * item.Quantity;

                var itemPriceDto = new ReservationItemPriceDto
                {
                    TicketTypeId = item.TicketTypeId,
                    TicketTypeName = ticketType.TypeName,
                    Quantity = item.Quantity,
                    UnitPrice = unitPrice,
                    DiscountAmount = 0, // 暂时不实现折扣
                    TotalAmount = subtotal
                };

                result.Items.Add(itemPriceDto);
                result.SubtotalAmount += subtotal;
            }

            // 3. 应用促销折扣（如果有）
            if (request.PromotionId.HasValue)
            {
                var promotion = await _context.Promotions
                    .Where(p => p.PromotionId == request.PromotionId.Value && p.IsActive)
                    .FirstOrDefaultAsync(cancellationToken);

                if (promotion != null)
                {
                    // 简化处理：假设是固定折扣
                    result.DiscountAmount = result.SubtotalAmount * 0.1m; // 10%折扣
                }
            }

            // 4. 计算最终总价
            result.TotalAmount = result.SubtotalAmount - result.DiscountAmount;

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating reservation price for visitor {VisitorId}", request.VisitorId);
            throw new InvalidOperationException("Failed to calculate reservation price", ex);
        }
    }
}

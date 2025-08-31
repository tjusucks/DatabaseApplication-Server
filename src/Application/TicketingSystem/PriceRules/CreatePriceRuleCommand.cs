using DbApp.Domain.Entities.TicketingSystem;
using DbApp.Domain.Interfaces.TicketingSystem;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using DbApp.Domain.Interfaces;

namespace DbApp.Application.TicketingSystem.PriceRules;

public class CreatePriceRuleCommand : IRequest<PriceRuleDto?>
{
    public int TicketTypeId { get; }
    public CreatePriceRuleRequest Dto { get; }

    public CreatePriceRuleCommand(int ticketTypeId, CreatePriceRuleRequest dto)
    {
        TicketTypeId = ticketTypeId;
        Dto = dto;
    }
}

public class CreatePriceRuleCommandHandler : IRequestHandler<CreatePriceRuleCommand, PriceRuleDto?>
{
    private readonly IPriceRuleRepository _priceRuleRepository;
    private readonly ITicketTypeRepository _ticketTypeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePriceRuleCommandHandler(IPriceRuleRepository priceRuleRepository, ITicketTypeRepository ticketTypeRepository, IUnitOfWork unitOfWork)
    {
        _priceRuleRepository = priceRuleRepository;
        _ticketTypeRepository = ticketTypeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PriceRuleDto?> Handle(CreatePriceRuleCommand request, CancellationToken cancellationToken)
    {
        var ticketType = await _ticketTypeRepository.GetByIdAsync(request.TicketTypeId);
        if (ticketType == null || request.Dto.Price < 0 || request.Dto.EffectiveStartDate >= request.Dto.EffectiveEndDate)
        {
            return null;
        }

        var priceRule = new PriceRule
        {
            TicketTypeId = request.TicketTypeId,
            RuleName = request.Dto.RuleName,
            Priority = request.Dto.Priority,
            Price = request.Dto.Price,
            EffectiveStartDate = request.Dto.EffectiveStartDate,
            EffectiveEndDate = request.Dto.EffectiveEndDate
        };

        var newRule = await _priceRuleRepository.AddAsync(priceRule);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new PriceRuleDto
        {
            Id = newRule.PriceRuleId,
            RuleName = newRule.RuleName,
            Priority = newRule.Priority,
            Price = newRule.Price,
            EffectiveStartDate = newRule.EffectiveStartDate,
            EffectiveEndDate = newRule.EffectiveEndDate
        };
    }
}
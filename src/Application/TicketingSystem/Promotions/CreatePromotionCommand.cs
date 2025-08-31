
using DbApp.Domain.Entities.TicketingSystem;
using DbApp.Domain.Interfaces;
using DbApp.Domain.Interfaces.TicketingSystem;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DbApp.Application.TicketingSystem.Promotions;

public class CreatePromotionCommand : IRequest<PromotionDetailDto>
{
    public CreatePromotionRequest Dto { get; }

    public CreatePromotionCommand(CreatePromotionRequest dto)
    {
        Dto = dto;
    }
}

public class CreatePromotionCommandHandler : IRequestHandler<CreatePromotionCommand, PromotionDetailDto>
{
    private readonly IPromotionRepository _promotionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator; // To re-fetch the detail after creation

    public CreatePromotionCommandHandler(IPromotionRepository promotionRepository, IUnitOfWork unitOfWork, IMediator mediator)
    {
        _promotionRepository = promotionRepository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task<PromotionDetailDto> Handle(CreatePromotionCommand request, CancellationToken cancellationToken)
    {
        var promotion = new Promotion
        {
            PromotionName = request.Dto.PromotionName,
            PromotionType = request.Dto.PromotionType,
            StartDatetime = request.Dto.StartDate,
            EndDatetime = request.Dto.EndDate,
            IsActive = true,
            PromotionTicketTypes = request.Dto.ApplicableTicketTypeIds.Select(ticketId => new PromotionTicketType { TicketTypeId = ticketId }).ToList(),
            PromotionConditions = request.Dto.Conditions.Select(cond => new PromotionCondition
            {
                ConditionName = cond.ConditionName,
                ConditionType = cond.ConditionType,
                TicketTypeId = cond.TicketTypeId,
                MinQuantity = cond.MinQuantity,
                MinAmount = cond.MinAmount,
                Priority = cond.Priority
            }).ToList(),
            PromotionActions = request.Dto.Actions.Select(act => new PromotionAction
            {
                ActionName = act.ActionName,
                ActionType = act.ActionType,
                DiscountPercentage = act.DiscountPercentage,
                DiscountAmount = act.DiscountAmount
            }).ToList()
        };
        
        var newPromotion = await _promotionRepository.AddAsync(promotion);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        // Fetch the full detail DTO to return to the client
        return await _mediator.Send(new GetPromotionDetailQuery(newPromotion.PromotionId), cancellationToken);
    }
}
using AutoMapper;
using DbApp.Domain.Interfaces.TicketingSystem;
using MediatR;

namespace DbApp.Application.TicketingSystem.PromotionConditions;

public class PromotionConditionQueryHandler(
    IPromotionConditionRepository conditionRepository,
    IMapper mapper
) :
    IRequestHandler<GetPromotionConditionByIdQuery, PromotionConditionDto?>,
    IRequestHandler<GetPromotionConditionsByPromotionIdQuery, List<PromotionConditionDto>>
{
    public async Task<PromotionConditionDto?> Handle(GetPromotionConditionByIdQuery request, CancellationToken cancellationToken)
    {
        var condition = await conditionRepository.GetByIdAsync(request.ConditionId);
        return condition == null ? null : mapper.Map<PromotionConditionDto>(condition);
    }

    public async Task<List<PromotionConditionDto>> Handle(GetPromotionConditionsByPromotionIdQuery request, CancellationToken cancellationToken)
    {
        var conditions = await conditionRepository.GetByPromotionIdAsync(request.PromotionId);
        return mapper.Map<List<PromotionConditionDto>>(conditions);
    }
}

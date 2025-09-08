using AutoMapper;
using DbApp.Domain.Interfaces.TicketingSystem;
using MediatR;
using static DbApp.Domain.Exceptions;

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
        var condition = await conditionRepository.GetByIdAsync(request.ConditionId)
            ?? throw new NotFoundException($"{request.ConditionId} could not be found");
        return mapper.Map<PromotionConditionDto>(condition);
    }

    public async Task<List<PromotionConditionDto>> Handle(GetPromotionConditionsByPromotionIdQuery request, CancellationToken cancellationToken)
    {
        var conditions = await conditionRepository.GetByPromotionIdAsync(request.PromotionId);
        return mapper.Map<List<PromotionConditionDto>>(conditions);
    }
}

using AutoMapper;
using DbApp.Domain.Entities.TicketingSystem;

namespace DbApp.Application.TicketingSystem.PromotionConditions;

public class PromotionConditionMappingProfile : Profile
{
    public PromotionConditionMappingProfile()
    {
        CreateMap<PromotionCondition, PromotionConditionDto>();
    }
}

using AutoMapper;
using DbApp.Domain.Entities.TicketingSystem;
using DbApp.Domain.Specifications.TicketingSystem;
using DbApp.Domain.Statistics.TicketingSystem;

namespace DbApp.Application.TicketingSystem.Tickets;

public class TicketSaleMappingProfile : Profile
{
    public TicketSaleMappingProfile()
    {
        // Query to Spec mappings
        CreateMap<SearchTicketSaleQuery, TicketSaleSearchSpec>();
        CreateMap<SearchTicketSaleQuery, TicketSaleCountSpec>();
        CreateMap<GetTicketSaleStatsQuery, TicketSaleStatsSpec>();
        CreateMap<GetGroupedTicketSaleStatsQuery, TicketSaleGroupedStatsSpec>();

        // Entity to DTO mappings
        CreateMap<Ticket, TicketSaleSummaryDto>()
            .ForMember(dest => dest.TicketTypeName, opt =>
                opt.MapFrom(src => src.TicketType.TypeName))
            .ForMember(dest => dest.VisitorName, opt =>
                opt.MapFrom(src => src.Visitor != null && src.Visitor.User != null
                    ? src.Visitor.User.Username
                    : null))
            .ForMember(dest => dest.VisitorEmail, opt =>
                opt.MapFrom(src => src.Visitor != null && src.Visitor.User != null
                    ? src.Visitor.User.Email
                    : null))
            .ForMember(dest => dest.PromotionName, opt =>
                opt.MapFrom(src => src.ReservationItem.Reservation.Promotion != null
                    ? src.ReservationItem.Reservation.Promotion.PromotionName
                    : null))
            .ForMember(dest => dest.SalesDate, opt =>
                opt.MapFrom(src => src.ReservationItem.Reservation.CreatedAt))
            .ForMember(dest => dest.VisitDate, opt =>
                opt.MapFrom(src => src.ReservationItem.Reservation.VisitDate))
            .ForMember(dest => dest.PaymentStatus, opt =>
                opt.MapFrom(src => src.ReservationItem.Reservation.PaymentStatus))
            .ForMember(dest => dest.Status, opt =>
                opt.MapFrom(src => src.Status));

        CreateMap<TicketSaleStats, TicketSaleStatsDto>();
        CreateMap<GroupedTicketSaleStats, GroupedTicketSaleStatsDto>();
    }
}

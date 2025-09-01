using AutoMapper;
using DbApp.Domain.Entities.TicketingSystem;
using DbApp.Domain.Specifications.TicketingSystem;
using DbApp.Domain.Statistics.TicketingSystem;

namespace DbApp.Application.TicketingSystem.Reservations;

/// <summary>
/// AutoMapper profile for mapping Reservation and related entities to Reservation DTOs.
/// </summary>
public class ReservationMappingProfile : Profile
{
    public ReservationMappingProfile()
    {
        // Query to Spec mappings.
        CreateMap<SearchReservationByVisitorQuery, ReservationSearchByVisitorSpec>();
        CreateMap<SearchReservationByVisitorQuery, ReservationCountByVisitorSpec>();
        CreateMap<SearchReservationQuery, ReservationSearchSpec>();
        CreateMap<SearchReservationQuery, ReservationCountSpec>();
        CreateMap<GetVisitorReservationStatsQuery, ReservationStatsByVisitorSpec>();

        // Entity to DTO mappings.
        CreateMap<Reservation, ReservationSummaryDto>()
            .ForMember(dest => dest.VisitorName, opt =>
                opt.MapFrom(src => src.Visitor != null ? src.Visitor.User.Username : string.Empty))
            .ForMember(dest => dest.VisitorEmail, opt =>
                opt.MapFrom(src => src.Visitor != null ? src.Visitor.User.Email : null))
            .ForMember(dest => dest.PromotionName, opt =>
                opt.MapFrom(src => src.Promotion != null ? src.Promotion.PromotionName : null))
            .ForMember(dest => dest.TotalTickets, opt =>
                opt.MapFrom(src => src.ReservationItems != null ? src.ReservationItems.Sum(i => i.Quantity) : 0));

        CreateMap<ReservationStats, ReservationStatsDto>();
    }
}

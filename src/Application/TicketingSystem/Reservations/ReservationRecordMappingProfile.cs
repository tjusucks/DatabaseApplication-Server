using AutoMapper;
using DbApp.Domain.Entities.TicketingSystem;
using DbApp.Domain.Statistics.TicketingSystem;

namespace DbApp.Application.TicketingSystem.Reservations;

/// <summary>
/// AutoMapper profile for mapping Reservation and related entities to ReservationRecord DTOs.
/// </summary>
public class ReservationRecordMappingProfile : Profile
{
    public ReservationRecordMappingProfile()
    {
        CreateMap<Reservation, ReservationRecordSummaryDto>()
            .ForMember(dest => dest.VisitorName, opt =>
                opt.MapFrom(src => src.Visitor != null ? src.Visitor.User.Username : string.Empty))
            .ForMember(dest => dest.VisitorEmail, opt =>
                opt.MapFrom(src => src.Visitor != null ? src.Visitor.User.Email : null))
            .ForMember(dest => dest.PromotionName, opt =>
                opt.MapFrom(src => src.Promotion != null ? src.Promotion.PromotionName : null))
            .ForMember(dest => dest.TotalTickets, opt =>
                opt.MapFrom(src => src.ReservationItems != null ? src.ReservationItems.Sum(i => i.Quantity) : 0));

        CreateMap<ReservationRecordStats, ReservationRecordStatsDto>();
    }
}

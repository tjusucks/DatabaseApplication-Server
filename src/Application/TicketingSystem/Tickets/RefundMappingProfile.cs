using AutoMapper;
using DbApp.Domain.Entities.TicketingSystem;
using DbApp.Domain.Interfaces.TicketingSystem;

namespace DbApp.Application.TicketingSystem.Tickets;

/// <summary>
/// 退票功能的AutoMapper配置
/// </summary>
public class RefundMappingProfile : Profile
{
    public RefundMappingProfile()
    {
        // RefundRecord -> RefundDetailsDto
        CreateMap<RefundRecord, RefundDetailsDto>()
            .ForMember(dest => dest.TicketSerialNumber, opt => opt.MapFrom(src => src.Ticket.SerialNumber))
            .ForMember(dest => dest.TicketTypeName, opt => opt.MapFrom(src => src.Ticket.TicketType.TypeName))
            .ForMember(dest => dest.VisitorName, opt => opt.MapFrom(src =>
                src.Visitor.User != null ? src.Visitor.User.DisplayName : "Unknown"))
            .ForMember(dest => dest.OriginalAmount, opt => opt.MapFrom(src =>
                src.Ticket.ReservationItem != null ? src.Ticket.ReservationItem.UnitPrice : 0))
            .ForMember(dest => dest.RefundFee, opt => opt.MapFrom(src =>
                src.Ticket.ReservationItem != null ?
                    src.Ticket.ReservationItem.UnitPrice - src.RefundAmount : 0))
            .ForMember(dest => dest.ProcessorName, opt => opt.MapFrom(src =>
                src.Processor != null && src.Processor.User != null ? src.Processor.User.DisplayName : null));

        // RefundStatistics -> RefundStatsDto
        CreateMap<RefundStatistics, RefundStatsDto>()
            .ForMember(dest => dest.NetRefundAmount, opt => opt.MapFrom(src =>
                src.TotalRefundAmount - src.TotalRefundFees));
    }
}

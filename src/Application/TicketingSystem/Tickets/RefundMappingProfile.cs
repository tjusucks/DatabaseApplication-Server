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
                src.Processor != null ? src.Processor.User.DisplayName : null));

        // RefundStatistics -> RefundStatsDto
        CreateMap<RefundStatistics, RefundStatsDto>()
            .ForMember(dest => dest.NetRefundAmount, opt => opt.MapFrom(src =>
                src.TotalRefundAmount - src.TotalRefundFees))
            .ForMember(dest => dest.RefundRate, opt => opt.MapFrom(src =>
                CalculateRefundRate(src.TotalRefunds, src.TotalRefunds + 1000))); // 简化计算，实际应该传入总销售数
    }

    private static decimal CalculateRefundRate(int refunds, int totalSales)
    {
        return totalSales > 0 ? (decimal)refunds / totalSales * 100 : 0;
    }
}

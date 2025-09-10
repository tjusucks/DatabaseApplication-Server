using AutoMapper;
using DbApp.Domain.Entities.UserSystem;

namespace DbApp.Application.UserSystem.RideEntryRecords;

/// <summary>
/// AutoMapper profile for RideEntryRecord entity and DTO mapping.
/// </summary>
public class RideEntryRecordMappingProfile : Profile
{
    public RideEntryRecordMappingProfile()
    {
        CreateMap<RideEntryRecord, RideEntryRecordDto>()
            .ForMember(dest => dest.VisitorName, opt => opt.MapFrom(src => src.Visitor.User.Username))
            .ForMember(dest => dest.RideName, opt => opt.MapFrom(src => src.Ride.RideName))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.ExitTime == null));
    }
}

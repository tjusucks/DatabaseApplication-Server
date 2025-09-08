using AutoMapper;
using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Statistics.ResourceSystem;

namespace DbApp.Application.ResourceSystem.MaintenanceRecords;

/// <summary>
/// AutoMapper profile for mapping MaintenanceRecord and related entities to MaintenanceRecord DTOs.
/// </summary>
public class MaintenanceRecordMappingProfile : Profile
{
    public MaintenanceRecordMappingProfile()
    {
        CreateMap<MaintenanceRecord, MaintenanceRecordSummaryDto>()
            .ForMember(dest => dest.RideName, opt =>
                opt.MapFrom(src => src.Ride != null ? src.Ride.RideName : string.Empty))
            .ForMember(dest => dest.TeamName, opt =>
                opt.MapFrom(src => src.Team != null ? src.Team.TeamName : string.Empty))
            .ForMember(dest => dest.ManagerName, opt =>
                opt.MapFrom(src => src.Manager != null ? src.Manager.User.Username : null));

        CreateMap<MaintenanceRecordStats, MaintenanceRecordStatsDto>();
    }
}

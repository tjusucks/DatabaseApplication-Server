using AutoMapper;  
using DbApp.Domain.Entities.ResourceSystem;  
using DbApp.Domain.Statistics.ResourceSystem;  
  
namespace DbApp.Application.ResourceSystem.RideTrafficStats;  
  
/// <summary>  
/// AutoMapper profile for mapping RideTrafficStat and related entities to RideTrafficStat DTOs.  
/// </summary>  
public class RideTrafficStatMappingProfile : Profile  
{  
    public RideTrafficStatMappingProfile()  
    {  
        CreateMap<RideTrafficStat, RideTrafficStatSummaryDto>()  
            .ForMember(dest => dest.RideName, opt =>  
                opt.MapFrom(src => src.Ride != null ? src.Ride.RideName : string.Empty));  
                   
        CreateMap<DbApp.Domain.Statistics.ResourceSystem.RideTrafficStats, RideTrafficStatsDto>();  
    }  
}
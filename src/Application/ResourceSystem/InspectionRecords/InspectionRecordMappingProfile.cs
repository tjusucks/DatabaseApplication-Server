using AutoMapper;  
using DbApp.Domain.Entities.ResourceSystem;  
  
namespace DbApp.Application.ResourceSystem.InspectionRecords;  
  
/// <summary>  
/// AutoMapper profile for mapping InspectionRecord and related entities to DTOs.  
/// </summary>  
public class InspectionRecordMappingProfile : Profile  
{  
    public InspectionRecordMappingProfile()  
    {  
        CreateMap<InspectionRecord, InspectionRecordSummaryDto>()  
            .ForMember(dest => dest.RideName, opt =>  
                opt.MapFrom(src => src.Ride != null ? src.Ride.RideName : string.Empty))  
            .ForMember(dest => dest.TeamName, opt =>  
                opt.MapFrom(src => src.Team != null ? src.Team.TeamName : string.Empty));  
    }  
}
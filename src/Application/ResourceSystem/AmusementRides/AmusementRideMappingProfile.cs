using AutoMapper;
using DbApp.Domain.Entities.ResourceSystem;

namespace DbApp.Application.ResourceSystem.AmusementRides;

/// <summary>  
/// AutoMapper profile for mapping AmusementRide and related entities to AmusementRide DTOs.  
/// </summary>  
public class AmusementRideMappingProfile : Profile
{
    public AmusementRideMappingProfile()
    {
        CreateMap<AmusementRide, AmusementRideSummaryDto>()
            .ForMember(dest => dest.ManagerName, opt =>
                opt.MapFrom(src => src.Manager != null ? src.Manager.User.Username : null));
    }
}

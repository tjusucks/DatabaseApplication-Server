using AutoMapper;
using DbApp.Domain.Entities.UserSystem;
using DbApp.Application.UserSystem.Users;
using DbApp.Domain.Statistics.UserSystem;
using DbApp.Domain.Specifications.UserSystem;
using DbApp.Domain.Specifications.Common;

namespace DbApp.Application.UserSystem.Visitors;

public class VisitorMappingProfile : Profile
{
    public VisitorMappingProfile()
    {
        // Entity to DTO mappings.
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.RoleName));

        CreateMap<Visitor, VisitorDto>();

        CreateMap<VisitorStats, VisitorStatsDto>();

        // Query to Spec mappings (for filtering/statistics).
        CreateMap<SearchVisitorsQuery, PaginatedSpec<VisitorSpec>>();
        CreateMap<GetVisitorStatsQuery, VisitorSpec>();
        CreateMap<GetGroupedVisitorStatsQuery, GroupedSpec<VisitorSpec>>();
    }
}

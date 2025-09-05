using AutoMapper;
using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Statistics.UserSystem;
using DbApp.Domain.Specifications.UserSystem;
using DbApp.Domain.Specifications.Common;

namespace DbApp.Application.UserSystem.EntryRecords;

public class EntryRecordMappingProfile : Profile
{
    public EntryRecordMappingProfile()
    {
        // Entity to DTO mappings.
        CreateMap<EntryRecord, EntryRecordDto>()
            .ForMember(dest => dest.VisitorName, opt => opt.MapFrom(src => src.Visitor.User.DisplayName))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => !src.ExitTime.HasValue));

        CreateMap<EntryRecordStats, EntryRecordStatsDto>();

        // Query to Spec mappings (for filtering/statistics).
        CreateMap<SearchEntryRecordsQuery, PaginatedSpec<EntryRecordSpec>>();
        CreateMap<GetEntryRecordStatsQuery, EntryRecordSpec>();
        CreateMap<GetGroupedEntryRecordStatsQuery, GroupedSpec<EntryRecordSpec>>();
    }
}

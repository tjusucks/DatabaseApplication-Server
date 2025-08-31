using AutoMapper;
using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Statistics.ResourceSystem;

namespace DbApp.Application.ResourceSystem.FinancialRecords;

public class FinancialRecordMappingProfile : Profile
{
    public FinancialRecordMappingProfile()
    {
        CreateMap<FinancialRecord, FinancialRecordSummaryDto>()
            .ForMember(dest => dest.ResponsibleEmployeeName, opt =>
                opt.MapFrom(src => src.ResponsibleEmployee != null
                    ? src.ResponsibleEmployee.User.DisplayName
                    : null))
            .ForMember(dest => dest.ApprovedByName, opt =>
                opt.MapFrom(src => src.ApprovedBy != null
                    ? src.ApprovedBy.User.DisplayName
                    : null));

        CreateMap<FinancialRecord, FinancialRecordDetailDto>()
            .ForMember(dest => dest.ResponsibleEmployeeName, opt =>
                opt.MapFrom(src => src.ResponsibleEmployee != null
                    ? src.ResponsibleEmployee.User.DisplayName
                    : null))
            .ForMember(dest => dest.ResponsibleEmployeeStaffNumber, opt =>
                opt.MapFrom(src => src.ResponsibleEmployee != null
                    ? src.ResponsibleEmployee.StaffNumber
                    : null))
            .ForMember(dest => dest.ApprovedByName, opt =>
                opt.MapFrom(src => src.ApprovedBy != null
                    ? src.ApprovedBy.User.DisplayName
                    : null))
            .ForMember(dest => dest.ApprovedByStaffNumber, opt =>
                opt.MapFrom(src => src.ApprovedBy != null
                    ? src.ApprovedBy.StaffNumber
                    : null));

        CreateMap<FinancialStats, FinancialStatsDto>();
        CreateMap<GroupedFinancialStats, GroupedFinancialStatsDto>();
        CreateMap<IncomeExpenseOverview, IncomeExpenseOverviewDto>();
    }
}

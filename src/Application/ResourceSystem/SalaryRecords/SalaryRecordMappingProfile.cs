using AutoMapper;
using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Statistics.ResourceSystem;

namespace DbApp.Application.ResourceSystem.SalaryRecords;

public class SalaryRecordMappingProfile : Profile
{
    public SalaryRecordMappingProfile()
    {
        CreateMap<SalaryRecord, SalaryRecordSummaryDto>()
            .ForMember(dest => dest.EmployeeName, opt =>
                opt.MapFrom(src => src.Employee != null && src.Employee.User != null
                    ? src.Employee.User.DisplayName
                    : string.Empty))
            .ForMember(dest => dest.StaffNumber, opt =>
                opt.MapFrom(src => src.Employee != null
                    ? src.Employee.StaffNumber
                    : string.Empty))
            .ForMember(dest => dest.Position, opt =>
                opt.MapFrom(src => src.Employee != null
                    ? src.Employee.Position
                    : null))
            .ForMember(dest => dest.DepartmentName, opt =>
                opt.MapFrom(src => src.Employee != null
                    ? src.Employee.DepartmentName
                    : null));

        CreateMap<SalaryRecord, SalaryRecordDetailDto>()
            .ForMember(dest => dest.EmployeeName, opt =>
                opt.MapFrom(src => src.Employee != null && src.Employee.User != null
                    ? src.Employee.User.DisplayName
                    : string.Empty))
            .ForMember(dest => dest.StaffNumber, opt =>
                opt.MapFrom(src => src.Employee != null
                    ? src.Employee.StaffNumber
                    : string.Empty))
            .ForMember(dest => dest.Position, opt =>
                opt.MapFrom(src => src.Employee != null
                    ? src.Employee.Position
                    : null))
            .ForMember(dest => dest.DepartmentName, opt =>
                opt.MapFrom(src => src.Employee != null
                    ? src.Employee.DepartmentName
                    : null))
            .ForMember(dest => dest.StaffType, opt =>
                opt.MapFrom(src => src.Employee != null
                    ? src.Employee.StaffType
                    : null))
            .ForMember(dest => dest.EmploymentStatus, opt =>
                opt.MapFrom(src => src.Employee != null
                    ? src.Employee.EmploymentStatus
                    : DbApp.Domain.Enums.UserSystem.EmploymentStatus.Active));

        CreateMap<SalaryStats, SalaryStatsDto>();
        CreateMap<GroupedSalaryStats, GroupedSalaryStatsDto>();
        CreateMap<EmployeeSalarySummary, EmployeeSalarySummaryDto>();
        CreateMap<MonthlySalaryReport, MonthlySalaryReportDto>();
    }
}

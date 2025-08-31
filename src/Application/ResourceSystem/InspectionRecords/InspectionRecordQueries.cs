using MediatR;  
using DbApp.Domain.Enums.ResourceSystem;  
  
namespace DbApp.Application.ResourceSystem.InspectionRecords;  
  
/// <summary>  
/// Query to get inspection record by ID.  
/// </summary>  
public record GetInspectionRecordByIdQuery(int InspectionId) : IRequest<InspectionRecordSummaryDto?>;  
  
/// <summary>  
/// Query to search inspection records with filtering options.  
/// </summary>  
public record SearchInspectionRecordsQuery(  
    string? SearchTerm,   
    int Page = 1,   
    int PageSize = 10  
) : IRequest<InspectionRecordResult>;  
  
/// <summary>  
/// Query to search inspection records by ride.  
/// </summary>  
public record SearchInspectionRecordsByRideQuery(  
    int RideId,   
    int Page = 1,   
    int PageSize = 10  
) : IRequest<InspectionRecordResult>;  
  
/// <summary>  
/// Query to get inspection record statistics.  
/// </summary>  
public record GetInspectionRecordStatsQuery(  
    DateTime? StartDate = null,   
    DateTime? EndDate = null  
) : IRequest<InspectionRecordStatsDto>;
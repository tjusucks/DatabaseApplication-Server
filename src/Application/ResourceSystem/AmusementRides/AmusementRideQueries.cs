using MediatR;  
using DbApp.Domain.Enums.ResourceSystem;  
  
namespace DbApp.Application.ResourceSystem.AmusementRides;  
  
/// <summary>  
/// Query to get amusement ride by ID.  
/// </summary>  
public record GetAmusementRideByIdQuery(int RideId) : IRequest<AmusementRideSummaryDto?>;  
  
/// <summary>  
/// Query to search amusement rides with filtering options.  
/// </summary>  
public record SearchAmusementRidesQuery(  
    string? SearchTerm,   
    int Page = 1,   
    int PageSize = 10  
) : IRequest<AmusementRideResult>;  
  
/// <summary>  
/// Query to search amusement rides by status.  
/// </summary>  
public record SearchAmusementRidesByStatusQuery(  
    RideStatus Status,   
    int Page = 1,   
    int PageSize = 10  
) : IRequest<AmusementRideResult>;  
  
/// <summary>  
/// Query to get amusement ride statistics.  
/// </summary>  
public record GetAmusementRideStatsQuery(  
    DateTime? StartDate = null,   
    DateTime? EndDate = null  
) : IRequest<AmusementRideStatsDto>;  
  
/// <summary>  
/// Command to create a new amusement ride.  
/// </summary>  
public record CreateAmusementRideCommand(  
    string RideName,  
    string Location,  
    string? Description,  
    RideStatus RideStatus,  
    int Capacity,  
    int Duration,  
    int HeightLimitMin,  
    int HeightLimitMax,  
    DateTime? OpenDate,  
    int? ManagerId  
) : IRequest<int>;  
  
/// <summary>  
/// Command to update an existing amusement ride.  
/// </summary>  
public record UpdateAmusementRideCommand(  
    int RideId,  
    string RideName,  
    string Location,  
    string? Description,  
    RideStatus RideStatus,  
    int Capacity,  
    int Duration,  
    int HeightLimitMin,  
    int HeightLimitMax,  
    DateTime? OpenDate,  
    int? ManagerId  
) : IRequest;  
  
/// <summary>  
/// Command to delete an amusement ride.  
/// </summary>  
public record DeleteAmusementRideCommand(int RideId) : IRequest<bool>;
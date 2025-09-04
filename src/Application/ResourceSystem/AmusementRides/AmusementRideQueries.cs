using DbApp.Domain.Enums.ResourceSystem;
using MediatR;

namespace DbApp.Application.ResourceSystem.AmusementRides;

/// <summary>  
/// Query to get amusement ride by ID.  
/// </summary>  
public record GetAmusementRideByIdQuery(int RideId) : IRequest<AmusementRideSummaryDto?>;

/// <summary>  
/// Unified query to search amusement rides with comprehensive filtering options.  
/// </summary>  
public record SearchAmusementRidesQuery(
    string? SearchTerm = null,
    RideStatus? Status = null,
    string? Location = null,
    int? ManagerId = null,
    int? MinCapacity = null,
    int? MaxCapacity = null,
    int? MinHeightLimit = null,
    int? MaxHeightLimit = null,
    DateTime? OpenDateFrom = null,
    DateTime? OpenDateTo = null,
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

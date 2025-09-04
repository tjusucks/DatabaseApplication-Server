using AutoMapper;
using DbApp.Domain.Interfaces.ResourceSystem;
using MediatR;

namespace DbApp.Application.ResourceSystem.AmusementRides;

/// <summary>  
/// Combined handler for all amusement ride search and statistics queries.  
/// </summary>  
public class AmusementRideQueryHandler(
    IAmusementRideRepository amusementRideRepository,
    IMapper mapper) :
    IRequestHandler<GetAmusementRideByIdQuery, AmusementRideSummaryDto?>,
    IRequestHandler<SearchAmusementRidesQuery, AmusementRideResult>,
    IRequestHandler<GetAmusementRideStatsQuery, AmusementRideStatsDto>,
    IRequestHandler<CreateAmusementRideCommand, int>,
    IRequestHandler<UpdateAmusementRideCommand>,
    IRequestHandler<DeleteAmusementRideCommand, bool>
{
    private readonly IAmusementRideRepository _amusementRideRepository = amusementRideRepository;
    private readonly IMapper _mapper = mapper;

    /// <summary>  
    /// Handle getting amusement ride by ID.  
    /// </summary>  
    public async Task<AmusementRideSummaryDto?> Handle(
        GetAmusementRideByIdQuery request,
        CancellationToken cancellationToken)
    {
        var ride = await _amusementRideRepository.GetByIdAsync(request.RideId);
        return ride == null ? null : _mapper.Map<AmusementRideSummaryDto>(ride);
    }

    /// <summary>  
    /// Handle searching amusement rides with comprehensive filtering options.  
    /// </summary>  
    public async Task<AmusementRideResult> Handle(
        SearchAmusementRidesQuery request,
        CancellationToken cancellationToken)
    {
        var rides = await _amusementRideRepository.SearchAsync(
            request.SearchTerm,
            request.Status,
            request.Location,
            request.ManagerId,
            request.MinCapacity,
            request.MaxCapacity,
            request.MinHeightLimit,
            request.MaxHeightLimit,
            request.OpenDateFrom,
            request.OpenDateTo,
            request.Page,
            request.PageSize);

        var totalCount = await _amusementRideRepository.CountAsync(
            request.SearchTerm,
            request.Status,
            request.Location,
            request.ManagerId,
            request.MinCapacity,
            request.MaxCapacity,
            request.MinHeightLimit,
            request.MaxHeightLimit,
            request.OpenDateFrom,
            request.OpenDateTo);

        var rideDtos = _mapper.Map<List<AmusementRideSummaryDto>>(rides);

        return new AmusementRideResult
        {
            AmusementRides = rideDtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    /// <summary>  
    /// Handle getting amusement ride statistics.  
    /// </summary>  
    public async Task<AmusementRideStatsDto> Handle(
        GetAmusementRideStatsQuery request,
        CancellationToken cancellationToken)
    {
        var stats = await _amusementRideRepository.GetStatsAsync(
            request.StartDate,
            request.EndDate);

        return _mapper.Map<AmusementRideStatsDto>(stats);
    }

    /// <summary>  
    /// Handle creating a new amusement ride.  
    /// </summary>  
    public async Task<int> Handle(
        CreateAmusementRideCommand request,
        CancellationToken cancellationToken)
    {
        var ride = new Domain.Entities.ResourceSystem.AmusementRide
        {
            RideName = request.RideName,
            Location = request.Location,
            Description = request.Description,
            RideStatus = request.RideStatus,
            Capacity = request.Capacity,
            Duration = request.Duration,
            HeightLimitMin = request.HeightLimitMin,
            HeightLimitMax = request.HeightLimitMax,
            OpenDate = request.OpenDate,
            ManagerId = request.ManagerId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _amusementRideRepository.AddAsync(ride);
        return ride.RideId;
    }

    /// <summary>  
    /// Handle updating an existing amusement ride.  
    /// </summary>  
    public async Task Handle(
        UpdateAmusementRideCommand request,
        CancellationToken cancellationToken)
    {
        var ride = await _amusementRideRepository.GetByIdAsync(request.RideId);

        if (ride == null)
        {
            throw new InvalidOperationException("Amusement ride not found");
        }

        ride.RideName = request.RideName;
        ride.Location = request.Location;
        ride.Description = request.Description;
        ride.RideStatus = request.RideStatus;
        ride.Capacity = request.Capacity;
        ride.Duration = request.Duration;
        ride.HeightLimitMin = request.HeightLimitMin;
        ride.HeightLimitMax = request.HeightLimitMax;
        ride.OpenDate = request.OpenDate;
        ride.ManagerId = request.ManagerId;
        ride.UpdatedAt = DateTime.UtcNow;

        await _amusementRideRepository.UpdateAsync(ride);
    }

    /// <summary>  
    /// Handle deleting an amusement ride.  
    /// </summary>  
    public async Task<bool> Handle(
        DeleteAmusementRideCommand request,
        CancellationToken cancellationToken)
    {
        var ride = await _amusementRideRepository.GetByIdAsync(request.RideId);

        if (ride == null)
        {
            return false;
        }

        await _amusementRideRepository.DeleteAsync(ride);
        return true;
    }
}

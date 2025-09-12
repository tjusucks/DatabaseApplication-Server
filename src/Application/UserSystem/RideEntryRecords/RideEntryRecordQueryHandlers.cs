using AutoMapper;
using DbApp.Domain.Interfaces.UserSystem;
using MediatR;
using static DbApp.Domain.Exceptions;

namespace DbApp.Application.UserSystem.RideEntryRecords;

/// <summary>
/// Handlers for ride entry record queries.
/// </summary>
public class RideEntryRecordQueryHandlers(
    IRideEntryRecordRepository rideEntryRecordRepository,
    IMapper mapper) :
    IRequestHandler<GetRideEntryRecordByIdQuery, RideEntryRecordDto?>,
    IRequestHandler<GetAllRideEntryRecordsQuery, List<RideEntryRecordDto>>
{
    private readonly IRideEntryRecordRepository _rideEntryRecordRepository = rideEntryRecordRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<RideEntryRecordDto?> Handle(GetRideEntryRecordByIdQuery request, CancellationToken cancellationToken)
    {
        var rideEntryRecord = await _rideEntryRecordRepository.GetByIdAsync(request.RideEntryRecordId)
            ?? throw new NotFoundException($"Ride entry record with ID {request.RideEntryRecordId} not found.");
        return _mapper.Map<RideEntryRecordDto>(rideEntryRecord);
    }

    public async Task<List<RideEntryRecordDto>> Handle(GetAllRideEntryRecordsQuery request, CancellationToken cancellationToken)
    {
        var rideEntryRecords = await _rideEntryRecordRepository.GetAllAsync();
        return _mapper.Map<List<RideEntryRecordDto>>(rideEntryRecords);
    }
}

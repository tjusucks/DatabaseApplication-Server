using MediatR;

namespace DbApp.Application.UserSystem.RideEntryRecords;

/// <summary>
/// Query to get a ride entry record by ID.
/// </summary>
public class GetRideEntryRecordByIdQuery : IRequest<RideEntryRecordDto?>
{
    public int RideEntryRecordId { get; set; }
}

/// <summary>
/// Query to get all ride entry records.
/// </summary>
public class GetAllRideEntryRecordsQuery : IRequest<List<RideEntryRecordDto>>
{
}

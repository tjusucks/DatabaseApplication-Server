using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Interfaces.ResourceSystem;
using MediatR;

namespace DbApp.Application.ResourceSystem.InspectionRecords;

public class GetInspectionRecordByIdQueryHandler(IInspectionRecordRepository repository)
    : IRequestHandler<GetInspectionRecordByIdQuery, InspectionRecord?>
{
    public async Task<InspectionRecord?> Handle(GetInspectionRecordByIdQuery request, CancellationToken cancellationToken)
    {
        return await repository.GetByIdAsync(request.InspectionId);
    }
}

public class GetAllInspectionRecordsQueryHandler(IInspectionRecordRepository repository)
    : IRequestHandler<GetAllInspectionRecordsQuery, List<InspectionRecord>>
{
    public async Task<List<InspectionRecord>> Handle(GetAllInspectionRecordsQuery request, CancellationToken cancellationToken)
    {
        return await repository.GetAllAsync();
    }
}

public class GetInspectionRecordsByRideQueryHandler(IInspectionRecordRepository repository)
    : IRequestHandler<GetInspectionRecordsByRideQuery, List<InspectionRecord>>
{
    public async Task<List<InspectionRecord>> Handle(GetInspectionRecordsByRideQuery request, CancellationToken cancellationToken)
    {
        return await repository.GetByRideIdAsync(request.RideId);
    }
}

public class GetFailedInspectionsQueryHandler(IInspectionRecordRepository repository)
    : IRequestHandler<GetFailedInspectionsQuery, List<InspectionRecord>>
{
    public async Task<List<InspectionRecord>> Handle(GetFailedInspectionsQuery request, CancellationToken cancellationToken)
    {
        return await repository.GetFailedInspectionsAsync();
    }
}

public class GetInspectionRecordsByTypeQueryHandler(IInspectionRecordRepository repository)
    : IRequestHandler<GetInspectionRecordsByTypeQuery, List<InspectionRecord>>
{
    public async Task<List<InspectionRecord>> Handle(GetInspectionRecordsByTypeQuery request, CancellationToken cancellationToken)
    {
        return await repository.GetByCheckTypeAsync(request.CheckType);
    }
}

public class GetInspectionRecordsByDateRangeQueryHandler(IInspectionRecordRepository repository)
    : IRequestHandler<GetInspectionRecordsByDateRangeQuery, List<InspectionRecord>>
{
    public async Task<List<InspectionRecord>> Handle(GetInspectionRecordsByDateRangeQuery request, CancellationToken cancellationToken)
    {
        return await repository.GetByDateRangeAsync(request.StartDate, request.EndDate);
    }
}

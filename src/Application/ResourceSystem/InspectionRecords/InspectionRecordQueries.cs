using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Enums.ResourceSystem;
using MediatR;

namespace DbApp.Application.ResourceSystem.InspectionRecords;

public record GetInspectionRecordByIdQuery(int InspectionId) : IRequest<InspectionRecord?>;

public record GetAllInspectionRecordsQuery() : IRequest<List<InspectionRecord>>;

public record GetInspectionRecordsByRideQuery(int RideId) : IRequest<List<InspectionRecord>>;

public record GetFailedInspectionsQuery() : IRequest<List<InspectionRecord>>;

public record GetInspectionRecordsByTypeQuery(CheckType CheckType) : IRequest<List<InspectionRecord>>;

public record GetInspectionRecordsByDateRangeQuery(DateTime StartDate, DateTime EndDate) : IRequest<List<InspectionRecord>>;

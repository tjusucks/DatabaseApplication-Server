using DbApp.Domain.Enums.ResourceSystem;
using MediatR;

namespace DbApp.Application.ResourceSystem.InspectionRecords;

public record CreateInspectionRecordCommand(
    int RideId,
    int TeamId,
    DateTime CheckDate,
    CheckType CheckType,
    bool IsPassed,
    string? IssuesFound,
    string? Recommendations
) : IRequest<int>;

public record UpdateInspectionRecordCommand(
    int InspectionId,
    int RideId,
    int TeamId,
    DateTime CheckDate,
    CheckType CheckType,
    bool IsPassed,
    string? IssuesFound,
    string? Recommendations
) : IRequest<Unit>;

public record CompleteInspectionCommand(
    int InspectionId,
    bool IsPassed,
    string? IssuesFound,
    string? Recommendations
) : IRequest<Unit>;

public record DeleteInspectionRecordCommand(int InspectionId) : IRequest<Unit>;

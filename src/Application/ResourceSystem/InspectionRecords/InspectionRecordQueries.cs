using DbApp.Domain.Enums.ResourceSystem;
using MediatR;

namespace DbApp.Application.ResourceSystem.InspectionRecords;

/// <summary>
/// Query to get inspection record by ID.
/// </summary>
public record GetInspectionRecordByIdQuery(int InspectionId) : IRequest<InspectionRecordSummaryDto?>;

/// <summary>
/// Unified query to search inspection records with comprehensive filtering options.
/// </summary>
public record SearchInspectionRecordsQuery(
    string? Keyword = null,
    int? RideId = null,
    int? TeamId = null,
    CheckType? CheckType = null,
    bool? IsPassed = null,
    DateTime? CheckDateFrom = null,
    DateTime? CheckDateTo = null,
    int Page = 1,
    int PageSize = 10
) : IRequest<InspectionRecordResult>;

/// <summary>
/// Query to get inspection record statistics.
/// </summary>
public record GetInspectionRecordStatsQuery(
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    int? RideId = null
) : IRequest<InspectionRecordStatsDto>;

/// <summary>
/// Command to create a new inspection record.
/// </summary>
public record CreateInspectionRecordCommand(
    int RideId,
    int TeamId,
    DateTime CheckDate,
    CheckType CheckType,
    bool IsPassed,
    string? IssuesFound,
    string? Recommendations
) : IRequest<int>;

/// <summary>
/// Command to update an existing inspection record.
/// </summary>
public record UpdateInspectionRecordCommand(
    int InspectionId,
    int RideId,
    int TeamId,
    DateTime CheckDate,
    CheckType CheckType,
    bool IsPassed,
    string? IssuesFound,
    string? Recommendations
) : IRequest;

/// <summary>
/// Command to delete an inspection record.
/// </summary>
public record DeleteInspectionRecordCommand(int InspectionId) : IRequest<bool>;

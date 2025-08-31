using System.ComponentModel.DataAnnotations;
using DbApp.Domain.Enums.ResourceSystem;
using MediatR;

namespace DbApp.Application.ResourceSystem.FinancialRecords;

/// <summary>
/// Command to create a new financial record.
/// </summary>
public record CreateFinancialRecordCommand(
    DateTime TransactionDate,
    [Range(0.01, double.MaxValue)] decimal Amount,
    TransactionType TransactionType,
    PaymentMethod? PaymentMethod = null,
    int? ResponsibleEmployeeId = null,
    int? ApprovedById = null,
    string? Description = null
) : IRequest<FinancialRecordDetailDto>;

/// <summary>
/// Command to update an existing financial record.
/// </summary>
public record UpdateFinancialRecordCommand(
    int RecordId,
    DateTime TransactionDate,
    [Range(0.01, double.MaxValue)] decimal Amount,
    TransactionType TransactionType,
    PaymentMethod? PaymentMethod = null,
    int? ResponsibleEmployeeId = null,
    int? ApprovedById = null,
    string? Description = null
) : IRequest<FinancialRecordDetailDto?>;

/// <summary>
/// Command to delete a financial record.
/// </summary>
public record DeleteFinancialRecordCommand(
    int RecordId
) : IRequest<bool>;

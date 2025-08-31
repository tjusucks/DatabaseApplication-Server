using System.ComponentModel.DataAnnotations;
using MediatR;

namespace DbApp.Application.ResourceSystem.SalaryRecords;

/// <summary>
/// Command to create a new salary record.
/// </summary>
public record CreateSalaryRecordCommand(
    int EmployeeId,
    DateTime PayDate,
    [Range(0.01, double.MaxValue)] decimal Salary,
    string? Notes = null
) : IRequest<SalaryRecordDetailDto>;

/// <summary>
/// Command to update an existing salary record.
/// </summary>
public record UpdateSalaryRecordCommand(
    int SalaryRecordId,
    int EmployeeId,
    DateTime PayDate,
    [Range(0.01, double.MaxValue)] decimal Salary,
    string? Notes = null
) : IRequest<SalaryRecordDetailDto?>;

/// <summary>
/// Command to delete a salary record.
/// </summary>
public record DeleteSalaryRecordCommand(
    int SalaryRecordId
) : IRequest<bool>;

/// <summary>
/// Command to create salary records for multiple employees (batch payroll).
/// </summary>
public record CreateBatchSalaryRecordsCommand(
    DateTime PayDate,
    List<BatchSalaryItem> SalaryItems
) : IRequest<List<SalaryRecordDetailDto>>;

/// <summary>
/// Individual salary item for batch processing.
/// </summary>
public record BatchSalaryItem(
    int EmployeeId,
    [Range(0.01, double.MaxValue)] decimal Salary,
    string? Notes = null
);

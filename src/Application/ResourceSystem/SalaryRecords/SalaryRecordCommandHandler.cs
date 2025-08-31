using AutoMapper;
using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Interfaces.ResourceSystem;
using MediatR;

namespace DbApp.Application.ResourceSystem.SalaryRecords;

public class SalaryRecordCommandHandler(
    ISalaryRecordRepository salaryRecordRepository,
    IMapper mapper) :
    IRequestHandler<CreateSalaryRecordCommand, SalaryRecordDetailDto>,
    IRequestHandler<UpdateSalaryRecordCommand, SalaryRecordDetailDto?>,
    IRequestHandler<DeleteSalaryRecordCommand, bool>,
    IRequestHandler<CreateBatchSalaryRecordsCommand, List<SalaryRecordDetailDto>>
{
    private readonly ISalaryRecordRepository _salaryRecordRepository = salaryRecordRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<SalaryRecordDetailDto> Handle(CreateSalaryRecordCommand request, CancellationToken cancellationToken)
    {
        var salaryRecord = new SalaryRecord
        {
            EmployeeId = request.EmployeeId,
            PayDate = request.PayDate,
            Salary = request.Salary,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdRecord = await _salaryRecordRepository.AddAsync(salaryRecord);
        
        // Retrieve the full record with navigation properties
        var fullRecord = await _salaryRecordRepository.GetByIdAsync(createdRecord.SalaryRecordId);
        
        return _mapper.Map<SalaryRecordDetailDto>(fullRecord);
    }

    public async Task<SalaryRecordDetailDto?> Handle(UpdateSalaryRecordCommand request, CancellationToken cancellationToken)
    {
        var existingRecord = await _salaryRecordRepository.GetByIdAsync(request.SalaryRecordId);
        if (existingRecord == null)
        {
            return null;
        }

        existingRecord.EmployeeId = request.EmployeeId;
        existingRecord.PayDate = request.PayDate;
        existingRecord.Salary = request.Salary;
        existingRecord.Notes = request.Notes;
        existingRecord.UpdatedAt = DateTime.UtcNow;

        var updatedRecord = await _salaryRecordRepository.UpdateAsync(existingRecord);
        
        // Retrieve the full record with navigation properties
        var fullRecord = await _salaryRecordRepository.GetByIdAsync(updatedRecord.SalaryRecordId);
        
        return _mapper.Map<SalaryRecordDetailDto>(fullRecord);
    }

    public async Task<bool> Handle(DeleteSalaryRecordCommand request, CancellationToken cancellationToken)
    {
        return await _salaryRecordRepository.DeleteAsync(request.SalaryRecordId);
    }

    public async Task<List<SalaryRecordDetailDto>> Handle(CreateBatchSalaryRecordsCommand request, CancellationToken cancellationToken)
    {
        var salaryRecords = request.SalaryItems.Select(item => new SalaryRecord
        {
            EmployeeId = item.EmployeeId,
            PayDate = request.PayDate,
            Salary = item.Salary,
            Notes = item.Notes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        }).ToList();

        var createdRecords = await _salaryRecordRepository.AddBatchAsync(salaryRecords);
        
        // Retrieve the full records with navigation properties
        var fullRecords = new List<SalaryRecord>();
        foreach (var record in createdRecords)
        {
            var fullRecord = await _salaryRecordRepository.GetByIdAsync(record.SalaryRecordId);
            if (fullRecord != null)
            {
                fullRecords.Add(fullRecord);
            }
        }
        
        return _mapper.Map<List<SalaryRecordDetailDto>>(fullRecords);
    }
}

using AutoMapper;
using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Interfaces.ResourceSystem;
using MediatR;

namespace DbApp.Application.ResourceSystem.FinancialRecords;

public class FinancialRecordCommandHandler(
    IFinancialRecordRepository financialRecordRepository,
    IMapper mapper) :
    IRequestHandler<CreateFinancialRecordCommand, FinancialRecordDetailDto>,
    IRequestHandler<UpdateFinancialRecordCommand, FinancialRecordDetailDto?>,
    IRequestHandler<DeleteFinancialRecordCommand, bool>
{
    private readonly IFinancialRecordRepository _financialRecordRepository = financialRecordRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<FinancialRecordDetailDto> Handle(CreateFinancialRecordCommand request, CancellationToken cancellationToken)
    {
        var financialRecord = new FinancialRecord
        {
            TransactionDate = request.TransactionDate,
            Amount = request.Amount,
            TransactionType = request.TransactionType,
            PaymentMethod = request.PaymentMethod,
            ResponsibleEmployeeId = request.ResponsibleEmployeeId,
            ApprovedById = request.ApprovedById,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdRecord = await _financialRecordRepository.AddAsync(financialRecord);
        
        // Retrieve the full record with navigation properties
        var fullRecord = await _financialRecordRepository.GetByIdAsync(createdRecord.RecordId);
        
        return _mapper.Map<FinancialRecordDetailDto>(fullRecord);
    }

    public async Task<FinancialRecordDetailDto?> Handle(UpdateFinancialRecordCommand request, CancellationToken cancellationToken)
    {
        var existingRecord = await _financialRecordRepository.GetByIdAsync(request.RecordId);
        if (existingRecord == null)
        {
            return null;
        }

        existingRecord.TransactionDate = request.TransactionDate;
        existingRecord.Amount = request.Amount;
        existingRecord.TransactionType = request.TransactionType;
        existingRecord.PaymentMethod = request.PaymentMethod;
        existingRecord.ResponsibleEmployeeId = request.ResponsibleEmployeeId;
        existingRecord.ApprovedById = request.ApprovedById;
        existingRecord.UpdatedAt = DateTime.UtcNow;

        var updatedRecord = await _financialRecordRepository.UpdateAsync(existingRecord);
        
        // Retrieve the full record with navigation properties
        var fullRecord = await _financialRecordRepository.GetByIdAsync(updatedRecord.RecordId);
        
        return _mapper.Map<FinancialRecordDetailDto>(fullRecord);
    }

    public async Task<bool> Handle(DeleteFinancialRecordCommand request, CancellationToken cancellationToken)
    {
        return await _financialRecordRepository.DeleteAsync(request.RecordId);
    }
}

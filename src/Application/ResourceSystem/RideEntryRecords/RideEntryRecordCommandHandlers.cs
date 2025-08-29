using DbApp.Domain.Entities.ResourceSystem;  
using DbApp.Domain.Interfaces.ResourceSystem;  
using MediatR;  
  
namespace DbApp.Application.ResourceSystem.RideEntryRecords;  
  
public class CreateRideEntryRecordCommandHandler(IRideEntryRecordRepository repository)   
    : IRequestHandler<CreateRideEntryRecordCommand, RideEntryRecord>  
{  
    private readonly IRideEntryRecordRepository _repository = repository;  
  
    public async Task<RideEntryRecord> Handle(CreateRideEntryRecordCommand request, CancellationToken cancellationToken)  
    {  
        var record = new RideEntryRecord  
        {  
            RideId = request.RideId,  
            VisitorId = request.VisitorId,  
            EntryTime = request.EntryTime,  
            TicketId = request.TicketId,  
            CreatedAt = DateTime.UtcNow  
        };  
  
        return await _repository.CreateAsync(record);  
    }  
}  
  
public class UpdateRideEntryRecordCommandHandler(IRideEntryRecordRepository repository)   
    : IRequestHandler<UpdateRideEntryRecordCommand>  
{  
    private readonly IRideEntryRecordRepository _repository = repository;  
  
    public async Task Handle(UpdateRideEntryRecordCommand request, CancellationToken cancellationToken)  
    {  
        var record = await _repository.GetByIdAsync(request.RideEntryRecordId)  
            ?? throw new InvalidOperationException("RideEntryRecord not found");  
  
        record.RideId = request.RideId;  
        record.VisitorId = request.VisitorId;  
        record.EntryTime = request.EntryTime;  
        record.ExitTime = request.ExitTime;  
        record.TicketId = request.TicketId;  
        record.UpdatedAt = DateTime.UtcNow;  
  
        await _repository.UpdateAsync(record);  
    }  
}  
  
public class SetExitTimeCommandHandler(IRideEntryRecordRepository repository)   
    : IRequestHandler<SetExitTimeCommand>  
{  
    private readonly IRideEntryRecordRepository _repository = repository;  
  
    public async Task Handle(SetExitTimeCommand request, CancellationToken cancellationToken)  
    {  
        var record = await _repository.GetByIdAsync(request.RideEntryRecordId)  
            ?? throw new InvalidOperationException("RideEntryRecord not found");  
  
        record.ExitTime = request.ExitTime;  
        record.UpdatedAt = DateTime.UtcNow;  
  
        await _repository.UpdateAsync(record);  
    }  
}  
  
public class DeleteRideEntryRecordCommandHandler(IRideEntryRecordRepository repository)   
    : IRequestHandler<DeleteRideEntryRecordCommand>  
{  
    private readonly IRideEntryRecordRepository _repository = repository;  
  
    public async Task Handle(DeleteRideEntryRecordCommand request, CancellationToken cancellationToken)  
    {  
        var record = await _repository.GetByIdAsync(request.RideEntryRecordId)  
            ?? throw new InvalidOperationException("RideEntryRecord not found");  
          
        await _repository.DeleteAsync(record);  
    }  
}
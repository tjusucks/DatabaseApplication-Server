using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Enums.ResourceSystem;
using DbApp.Domain.Interfaces.ResourceSystem;
using DbApp.Domain.Interfaces.UserSystem;
using static DbApp.Domain.Exceptions;

namespace DbApp.Infrastructure.Services.UserSystem;

/// <summary>
/// Service implementation for RideEntryRecord operations.
/// Provides business logic for visitor ride entry/exit management.
/// </summary>
public class RideEntryRecordService(
    IRideEntryRecordRepository rideEntryRecordRepository,
    IVisitorRepository visitorRepository,
    IAmusementRideRepository rideRepository,
    IRideTrafficStatService rideTrafficStatService,
    ApplicationDbContext dbContext) : IRideEntryRecordService
{
    private readonly IRideEntryRecordRepository _rideEntryRecordRepo = rideEntryRecordRepository;
    private readonly IVisitorRepository _visitorRepo = visitorRepository;
    private readonly IAmusementRideRepository _rideRepo = rideRepository;
    private readonly IRideTrafficStatService _rideTrafficStatService = rideTrafficStatService;
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<int> CreateRideEntryAsync(int visitorId, int rideId, string gateName, int? ticketId)
    {
        await _dbContext.Database.BeginTransactionAsync();
        try
        {
            // Validate visitor exists and is not blacklisted
            var visitor = await _visitorRepo.GetByIdAsync(visitorId)
                ?? throw new ValidationException("Visitor not found.");

            if (visitor.IsBlacklisted)
            {
                throw new ForbiddenException("Blacklisted visitors cannot enter rides.");
            }

            // Validate ride exists and is operational
            var ride = await _rideRepo.GetByIdAsync(rideId)
                ?? throw new ValidationException("Ride not found.");

            if (ride.RideStatus != RideStatus.Operating)
            {
                throw new ValidationException("Ride is not operational.");
            }

            var rideEntryRecord = new RideEntryRecord
            {
                VisitorId = visitorId,
                RideId = rideId,
                EntryTime = DateTime.UtcNow,
                EntryGate = gateName,
                TicketId = ticketId
            };

            var entryRecordId = await _rideEntryRecordRepo.CreateAsync(rideEntryRecord);
            
            // Update real-time traffic statistics for ride entry
            await _rideTrafficStatService.UpdateOnRideEntryAsync(rideId);
            
            await _dbContext.Database.CommitTransactionAsync();
            return entryRecordId;
        }
        catch
        {
            await _dbContext.Database.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<int> CreateRideExitAsync(int visitorId, int rideId, string gateName)
    {
        await _dbContext.Database.BeginTransactionAsync();
        try
        {
            // Find active ride entry record and update with exit information
            var activeEntry = await _rideEntryRecordRepo.GetActiveEntry(visitorId, rideId)
                ?? throw new ValidationException($"No active ride entry found for visitor {visitorId} on ride {rideId}.");

            activeEntry.ExitTime = DateTime.UtcNow;
            activeEntry.ExitGate = gateName;
            await _rideEntryRecordRepo.UpdateAsync(activeEntry);
            
            // Update real-time traffic statistics for ride exit
            await _rideTrafficStatService.UpdateOnRideExitAsync(rideId);
            
            await _dbContext.Database.CommitTransactionAsync();
            return activeEntry.EntryRecordId;
        }
        catch
        {
            await _dbContext.Database.RollbackTransactionAsync();
            throw;
        }
    }
}

using DbApp.Domain.Constants.UserSystem;
using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Enums.UserSystem;
using DbApp.Domain.Interfaces.UserSystem;
using DbApp.Domain.Specifications.UserSystem;
using static DbApp.Domain.Exceptions;

namespace DbApp.Infrastructure.Services.UserSystem;

public class EntryRecordService(
    IEntryRecordRepository entryRecordRepository,
    IVisitorRepository visitorRepository,
    IMembershipService membershipService,
    ApplicationDbContext dbContext) : IEntryRecordService
{
    private readonly IEntryRecordRepository _entryRecordRepo = entryRecordRepository;
    private readonly IVisitorRepository _visitorRepo = visitorRepository;
    private readonly IMembershipService _membershipService = membershipService;
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<int> CreateEntryAsync(int visitorId, string gateName, int? ticketId)
    {
        await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var entryRecord = new EntryRecord
            {
                VisitorId = visitorId,
                EntryTime = DateTime.UtcNow,
                EntryGate = gateName,
                TicketId = ticketId
            };


            var visitor = await _visitorRepo.GetByIdAsync(visitorId)
                ?? throw new ValidationException("Visitor not found.");

            if (visitor.IsBlacklisted)
            {
                throw new ForbiddenException("Blacklisted visitors cannot enter the park.");
            }

            await _entryRecordRepo.CreateAsync(entryRecord);
            if (visitor.VisitorType == VisitorType.Member)
            {
                int points = MembershipConstants.GetPointsEarningForActivity("ParkEntry");
                if (visitor.User.BirthDate.HasValue &&
                    visitor.User.BirthDate.Value.Date == DateTime.UtcNow.Date)
                {
                    // Check if this is the first entry today for birthday bonus.
                    var today = DateTime.UtcNow.Date;
                    var spec = new EntryRecordSpec
                    {
                        VisitorId = visitorId,
                        EntryTimeStart = today,
                        EntryTimeEnd = today.AddDays(1).AddTicks(-1)
                    };
                    var count = await _entryRecordRepo.CountAsync(spec);
                    if (count == 1)
                    {
                        points += MembershipConstants.GetPointsEarningForActivity("BirthdayBonus");
                    }
                }

                await _membershipService.AddPointsAsync(visitorId, points);
            }

            await _dbContext.Database.CommitTransactionAsync();
            return entryRecord.EntryRecordId;
        }
        catch
        {
            await _dbContext.Database.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<int> CreateExitAsync(int visitorId, string gateName)
    {
        // Find active entry record and update with exit information.
        await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var activeEntry = await _entryRecordRepo.GetActiveEntryByVisitorIdAsync(visitorId)
                ?? throw new ValidationException($"No active entry found for visitor {visitorId}.");
            activeEntry.ExitTime = DateTime.UtcNow;
            activeEntry.ExitGate = gateName;
            await _entryRecordRepo.UpdateAsync(activeEntry);
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

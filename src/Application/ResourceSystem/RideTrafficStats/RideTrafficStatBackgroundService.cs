using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Interfaces.ResourceSystem;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DbApp.Application.ResourceSystem.RideTrafficStats;

/// <summary>  
/// 后台服务，用于自动处理游乐设施流量统计数据  
/// </summary>  
public class RideTrafficStatBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RideTrafficStatBackgroundService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(15);
    private readonly object _lockObject = new object();
    private List<int> _cachedOperationalRideIds = new List<int>();
    private DateTime _lastCacheUpdate = DateTime.MinValue;

    public RideTrafficStatBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<RideTrafficStatBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Traffic statistics background service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessTrafficStatsAsync();
                await Task.Delay(_interval, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing traffic statistics.");
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }

    private async Task ProcessTrafficStatsAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IRideTrafficStatRepository>();

        _logger.LogInformation("Starting traffic statistics data processing.");

        var currentTime = DateTime.UtcNow;
        var recordTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day,
            currentTime.Hour, (currentTime.Minute / 15) * 15, 0, DateTimeKind.Utc);

        // 获取运营中的游乐设施（使用缓存优化）  
        var operationalRideIds = await GetOperationalRideIdsAsync(scope);

        if (!operationalRideIds.Any())
        {
            _logger.LogInformation("No operational amusement rides found, skipping statistical processing.");
            return;
        }

        var processedCount = 0;
        var failedCount = 0;

        // 并发处理游乐设施统计  
        var semaphore = new SemaphoreSlim(5); // 限制并发数为5  
        var tasks = operationalRideIds.Select(async rideId =>
        {
            await semaphore.WaitAsync();
            try
            {
                await UpdateTrafficStatsAsync(repository, rideId, recordTime);
                Interlocked.Increment(ref processedCount);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to process statistics for amusement ride {RideId}.", rideId);
                Interlocked.Increment(ref failedCount);
            }
            finally
            {
                semaphore.Release();
            }
        });

        await Task.WhenAll(tasks);

        _logger.LogInformation("Traffic statistics processing completed. Successful: {ProcessedCount}, Failed: {FailedCount}, Total: {TotalCount}.",
            processedCount, failedCount, operationalRideIds.Count);
    }

    private async Task<List<int>> GetOperationalRideIdsAsync(IServiceScope scope)
    {
        // 缓存策略：每小时更新一次运营设施列表  
        lock (_lockObject)
        {
            if (DateTime.UtcNow - _lastCacheUpdate < TimeSpan.FromHours(1) && _cachedOperationalRideIds.Any())
            {
                return _cachedOperationalRideIds;
            }
        }

        var amusementRideRepository = scope.ServiceProvider.GetRequiredService<IAmusementRideRepository>();

        // 使用具体的查询参数避免二义性  
        var operationalRides = await GetOperationalRidesDirectly(amusementRideRepository);
        var rideIds = operationalRides.Select(r => r.RideId).ToList();

        lock (_lockObject)
        {
            _cachedOperationalRideIds = rideIds;
            _lastCacheUpdate = DateTime.UtcNow;
        }

        return rideIds;
    }

    /// <summary>  
    /// 直接调用仓储方法获取运营中的游乐设施，避免参数二义性  
    /// </summary>  
    private async Task<IEnumerable<AmusementRide>> GetOperationalRidesDirectly(IAmusementRideRepository repository)
    {
        // 创建明确的参数变量  
        var searchParameters = new
        {
            SearchTerm = (string?)null,
            Status = Domain.Enums.ResourceSystem.RideStatus.Operating,
            Location = (string?)null,
            ManagerId = (int?)null,
            MinCapacity = (int?)null,
            MaxCapacity = (int?)null,
            MinHeightLimit = (int?)null,
            MaxHeightLimit = (int?)null,
            OpenDateFrom = (DateTime?)null,
            OpenDateTo = (DateTime?)null,
            Page = 1,
            PageSize = int.MaxValue
        };

        return await repository.SearchAsync(
            searchParameters.SearchTerm,
            searchParameters.Status,
            searchParameters.Location,
            searchParameters.ManagerId,
            searchParameters.MinCapacity,
            searchParameters.MaxCapacity,
            searchParameters.MinHeightLimit,
            searchParameters.MaxHeightLimit,
            searchParameters.OpenDateFrom,
            searchParameters.OpenDateTo,
            searchParameters.Page,
            searchParameters.PageSize);
    }

    private async Task UpdateTrafficStatsAsync(IRideTrafficStatRepository repository, int rideId, DateTime recordTime)
    {
        // 使用 Upsert 模式避免竞态条件  
        var existingStat = await repository.GetByIdAsync(rideId, recordTime);
        var isNewRecord = existingStat == null;

        var stat = existingStat ?? new RideTrafficStat
        {
            RideId = rideId,
            RecordTime = recordTime
        };

        // 计算统计数据  
        stat.VisitorCount = CalculateVisitorCount(rideId, recordTime);
        stat.QueueLength = CalculateQueueLength(rideId, recordTime);
        stat.WaitingTime = CalculateWaitingTime(stat.QueueLength, rideId);
        stat.IsCrowded = DetermineCrowdedStatus(stat);

        if (isNewRecord)
        {
            await repository.AddAsync(stat);
            _logger.LogDebug("Created new traffic statistics record for amusement ride {RideId} at {RecordTime}.", rideId, recordTime);
        }
        else
        {
            await repository.UpdateAsync(stat);
            _logger.LogDebug("Updated traffic statistics for amusement ride {RideId} at {RecordTime}.", rideId, recordTime);
        }
    }

    public async Task ArchiveOldStatsAsync(DateTime cutoffDate)
    {
        using var scope = _serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IRideTrafficStatRepository>();

        var batchSize = 1000;
        var totalArchived = 0;

        while (true)
        {
            var oldStats = await GetOldStatsForArchiving(repository, cutoffDate, batchSize);

            if (!oldStats.Any())
                break;

            foreach (var stat in oldStats)
            {
                await repository.DeleteAsync(stat);
            }

            totalArchived += oldStats.Count();
        }

        _logger.LogInformation("Archived {ArchivedCount} old traffic statistics records.", totalArchived);
    }

    /// <summary>  
    /// 获取需要归档的旧统计数据，避免参数二义性  
    /// </summary>  
    private async Task<IEnumerable<RideTrafficStat>> GetOldStatsForArchiving(
        IRideTrafficStatRepository repository,
        DateTime cutoffDate,
        int batchSize)
    {
        // 创建明确的查询参数  
        var archiveParameters = new
        {
            SearchTerm = (string?)null,
            RideId = (int?)null,
            IsCrowded = (bool?)null,
            MinVisitorCount = (int?)null,
            MaxVisitorCount = (int?)null,
            MinQueueLength = (int?)null,
            MaxQueueLength = (int?)null,
            MinWaitingTime = (int?)null,
            MaxWaitingTime = (int?)null,
            RecordTimeFrom = (DateTime?)null,
            RecordTimeTo = cutoffDate,
            Page = 1,
            PageSize = batchSize
        };

        return await repository.SearchAsync(
            archiveParameters.SearchTerm,
            archiveParameters.RideId,
            archiveParameters.IsCrowded,
            archiveParameters.MinVisitorCount,
            archiveParameters.MaxVisitorCount,
            archiveParameters.MinQueueLength,
            archiveParameters.MaxQueueLength,
            archiveParameters.MinWaitingTime,
            archiveParameters.MaxWaitingTime,
            archiveParameters.RecordTimeFrom,
            archiveParameters.RecordTimeTo,
            archiveParameters.Page,
            archiveParameters.PageSize);
    }

    /// <summary>  
    /// 计算访客数量   
    /// </summary>  
    private static int CalculateVisitorCount(int rideId, DateTime recordTime)
    {
        var random = new Random(rideId + recordTime.GetHashCode());
        var hour = recordTime.Hour;
        var dayOfWeek = recordTime.DayOfWeek;

        var baseCount = random.Next(15, 45);

        // 时间段调整  
        var timeMultiplier = hour switch
        {
            >= 10 and <= 12 => 1.4,  // 上午高峰  
            >= 13 and <= 15 => 1.6,  // 下午高峰  
            >= 16 and <= 18 => 1.3,  // 傍晚  
            _ => 1.0
        };

        // 周末调整  
        var weekendMultiplier = (dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday) ? 1.2 : 1.0;

        return (int)(baseCount * timeMultiplier * weekendMultiplier);
    }

    /// <summary>  
    /// 计算队列长度  
    /// </summary>  
    private static int CalculateQueueLength(int rideId, DateTime recordTime)
    {
        var random = new Random(rideId + recordTime.GetHashCode() + 1);
        var hour = recordTime.Hour;

        var baseQueue = random.Next(5, 25);

        // 高峰期队列更长  
        var peakMultiplier = (hour >= 10 && hour <= 16) ? 1.5 : 1.0;

        return (int)(baseQueue * peakMultiplier);
    }

    /// <summary>  
    /// 等待时间计算  
    /// </summary>  
    private static int CalculateWaitingTime(int queueLength, int rideId)
    {
        var baseWaitPerPerson = 2 + (rideId % 3);

        // 考虑设施效率：热门设施处理更快  
        var efficiencyFactor = rideId % 10 < 5 ? 0.8 : 1.2; // 50%的设施效率更高  

        // 队列长度影响：队列越长，人均等待时间略增  
        var queueFactor = queueLength > 20 ? 1.2 : 1.0;

        var totalWaitTime = queueLength * baseWaitPerPerson * efficiencyFactor * queueFactor;

        return Math.Max(0, (int)Math.Ceiling(totalWaitTime));
    }

    /// <summary>  
    /// 综合拥挤判断逻辑  
    /// </summary>  
    private static bool DetermineCrowdedStatus(RideTrafficStat stat)
    {
        // 多维度判断拥挤状态  
        var visitorThreshold = 60;      // 访客数量阈值  
        var queueThreshold = 20;        // 队列长度阈值  
        var waitingTimeThreshold = 25;  // 等待时间阈值（分钟）  

        // 任意两个条件满足即认为拥挤  
        var conditions = new[]
        {
            stat.VisitorCount > visitorThreshold,
            stat.QueueLength > queueThreshold,
            stat.WaitingTime > waitingTimeThreshold
        };

        return conditions.Count(c => c) >= 2;
    }
}

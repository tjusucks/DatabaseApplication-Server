using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Enums.ResourceSystem;
using Microsoft.EntityFrameworkCore;

namespace DbApp.Infrastructure.DataSeedings.ResourceSystem;

public class AmusementRideDataSeeding : IDataSeeding
{
    public void Seed(DbContext dbContext)
    {
        if (dbContext.Set<AmusementRide>().Any())
        {
            return; // Data already seeded.
        }
        dbContext.Set<AmusementRide>().AddRange(GetDefaultAmusementRides());
        dbContext.SaveChanges();
    }

    public Task SeedAsync(DbContext dbContext)
    {
        if (dbContext.Set<AmusementRide>().Any())
        {
            return Task.CompletedTask; // Data already seeded.
        }
        dbContext.Set<AmusementRide>().AddRange(GetDefaultAmusementRides());
        return dbContext.SaveChangesAsync();
    }

    private static List<AmusementRide> GetDefaultAmusementRides()
    {
        var now = DateTime.UtcNow;
        return
        [
            new()
            {
                RideId = 1,
                RideName = "云霄过山车",
                Location = "冒险乐园",
                Description = "惊险刺激的高速过山车，体验极速飞驰的快感",
                RideStatus = RideStatus.Operating,
                Capacity = 32,
                Duration = 240,
                HeightLimitMin = 120,
                HeightLimitMax = 200,
                OpenDate = new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                RideId = 2,
                RideName = "梦幻摩天轮",
                Location = "浪漫城堡",
                Description = "浪漫的巨型摩天轮，可以俯瞰整个园区美景",
                RideStatus = RideStatus.Operating,
                Capacity = 48,
                Duration = 900,
                HeightLimitMin = 100,
                HeightLimitMax = 250,
                OpenDate = new DateTime(2025, 5, 15, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                RideId = 3,
                RideName = "旋转木马",
                Location = "童话世界",
                Description = "经典的旋转木马，适合全家人一起游玩",
                RideStatus = RideStatus.Operating,
                Capacity = 24,
                Duration = 180,
                HeightLimitMin = 80,
                HeightLimitMax = 200,
                OpenDate = new DateTime(2025, 4, 20, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                RideId = 4,
                RideName = "激流勇进",
                Location = "水上世界",
                Description = "刺激的水上漂流项目，感受激流冲击的快感",
                RideStatus = RideStatus.Operating,
                Capacity = 20,
                Duration = 300,
                HeightLimitMin = 110,
                HeightLimitMax = 190,
                OpenDate = new DateTime(2025, 7, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                RideId = 5,
                RideName = "海盗船",
                Location = "冒险乐园",
                Description = "大型摆锤式游乐设施，体验海盗船摇摆的刺激",
                RideStatus = RideStatus.Operating,
                Capacity = 40,
                Duration = 200,
                HeightLimitMin = 120,
                HeightLimitMax = 195,
                OpenDate = new DateTime(2025, 6, 10, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                RideId = 6,
                RideName = "碰碰车",
                Location = "欢乐广场",
                Description = "经典的碰碰车游戏，适合各个年龄段的游客",
                RideStatus = RideStatus.Operating,
                Capacity = 16,
                Duration = 240,
                HeightLimitMin = 90,
                HeightLimitMax = 200,
                OpenDate = new DateTime(2025, 4, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                RideId = 7,
                RideName = "跳楼机",
                Location = "极限挑战区",
                Description = "高空自由落体项目，挑战你的胆量极限",
                RideStatus = RideStatus.Operating,
                Capacity = 12,
                Duration = 120,
                HeightLimitMin = 140,
                HeightLimitMax = 200,
                OpenDate = new DateTime(2025, 8, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                RideId = 8,
                RideName = "太空飞车",
                Location = "未来世界",
                Description = "室内过山车，在黑暗中体验太空飞行的感觉",
                RideStatus = RideStatus.Operating,
                Capacity = 28,
                Duration = 280,
                HeightLimitMin = 115,
                HeightLimitMax = 195,
                OpenDate = new DateTime(2025, 9, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                RideId = 9,
                RideName = "旋转咖啡杯",
                Location = "童话世界",
                Description = "温和的旋转类游乐设施，适合儿童和家庭",
                RideStatus = RideStatus.Operating,
                Capacity = 32,
                Duration = 150,
                HeightLimitMin = 85,
                HeightLimitMax = 200,
                OpenDate = new DateTime(2025, 5, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                RideId = 10,
                RideName = "大摆锤",
                Location = "极限挑战区",
                Description = "大型摆锤设施，体验失重和超重的双重刺激",
                RideStatus = RideStatus.Operating,
                Capacity = 36,
                Duration = 180,
                HeightLimitMin = 130,
                HeightLimitMax = 200,
                OpenDate = new DateTime(2025, 7, 15, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                RideId = 11,
                RideName = "小火车",
                Location = "童话世界",
                Description = "环园观光小火车，适合全家人轻松游览",
                RideStatus = RideStatus.Operating,
                Capacity = 60,
                Duration = 600,
                HeightLimitMin = 70,
                HeightLimitMax = 250,
                OpenDate = new DateTime(2025, 4, 15, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                RideId = 12,
                RideName = "鬼屋探险",
                Location = "神秘地带",
                Description = "恐怖主题的室内探险项目，胆小者慎入",
                RideStatus = RideStatus.Operating,
                Capacity = 8,
                Duration = 360,
                HeightLimitMin = 120,
                HeightLimitMax = 200,
                OpenDate = new DateTime(2025, 10, 31, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                RideId = 13,
                RideName = "水上碰碰船",
                Location = "水上世界",
                Description = "水上版本的碰碰车，清凉刺激的夏日体验",
                RideStatus = RideStatus.Operating,
                Capacity = 20,
                Duration = 300,
                HeightLimitMin = 100,
                HeightLimitMax = 200,
                OpenDate = new DateTime(2025, 6, 20, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                RideId = 14,
                RideName = "空中飞椅",
                Location = "浪漫城堡",
                Description = "高空旋转飞椅，体验在空中飞翔的感觉",
                RideStatus = RideStatus.Operating,
                Capacity = 24,
                Duration = 200,
                HeightLimitMin = 110,
                HeightLimitMax = 190,
                OpenDate = new DateTime(2025, 5, 20, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                RideId = 15,
                RideName = "4D影院",
                Location = "未来世界",
                Description = "沉浸式4D电影体验，视觉听觉触觉的全方位享受",
                RideStatus = RideStatus.Operating,
                Capacity = 80,
                Duration = 900,
                HeightLimitMin = 90,
                HeightLimitMax = 220,
                OpenDate = new DateTime(2025, 8, 15, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = now,
                UpdatedAt = now
            }
        ];
    }
}

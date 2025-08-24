using DbApp.Domain.Entities.ResourceSystem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DbApp.Infrastructure.Configurations.ResourceSystem;

public class AmusementRideConfiguration : IEntityTypeConfiguration<AmusementRide>
{
    public void Configure(EntityTypeBuilder<AmusementRide> builder)
    {
        // 表名和基础配置
        builder.ToTable("AMUSEMENT_RIDES");
        builder.HasKey(r => r.RideId);

        // 属性映射
        builder.Property(r => r.RideId)
            .HasColumnName("RIDE_ID")
            .HasPrecision(10);

        builder.Property(r => r.RideName)
            .IsRequired()
            .HasColumnName("RIDE_NAME")
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.Property(r => r.ManagerId)
            .HasColumnName("MANAGER_ID")
            .HasPrecision(10);

        builder.Property(r => r.Location)
            .IsRequired()
            .HasColumnName("LOCATION")
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.Property(r => r.Description)
            .HasColumnName("DESCRIPTION")
            .IsUnicode(false);

        builder.Property(r => r.RideStatus)
            .IsRequired()
            .HasColumnName("RIDE_STATUS")
            .HasMaxLength(30)
            .IsUnicode(false)
            .HasConversion<string>();

        builder.Property(r => r.Capacity)
            .HasColumnName("CAPACITY")
            .HasPrecision(5);

        builder.Property(r => r.Duration)
            .HasColumnName("DURATION")
            .HasPrecision(5);

        builder.Property(r => r.HeightLimitMin)
            .HasColumnName("HEIGHT_LIMIT_MIN")
            .HasColumnType("decimal(5,2)");

        builder.Property(r => r.HeightLimitMax)
            .HasColumnName("HEIGHT_LIMIT_MAX")
            .HasColumnType("decimal(5,2)");

        builder.Property(r => r.OpenDate)
            .HasColumnName("OPEN_DATE");

        builder.Property(r => r.CreatedAt)
            .HasColumnName("CREATED_AT");

        builder.Property(r => r.UpdatedAt)
            .HasColumnName("UPDATED_AT");

        // 索引配置
        builder.HasIndex(r => r.ManagerId, "AMUSEMENT_RIDES_MANAGER_ID_IDX");
        builder.HasIndex(r => r.RideName, "AMUSEMENT_RIDES_RIDE_NAME_IDX");
        builder.HasIndex(r => r.RideStatus, "AMUSEMENT_RIDES_RIDE_STATUS_IDX");

        // 关系配置
        // 与管理员的关系
        builder.HasOne(r => r.Manager)
            .WithMany(e => e.AmusementRides)
            .HasForeignKey(r => r.ManagerId);

        // 集合关系配置
        builder.HasMany(r => r.InspectionRecords)
            .WithOne(i => i.Ride)
            .HasForeignKey(i => i.RideId);

        builder.HasMany(r => r.MaintenanceRecords)
            .WithOne(m => m.Ride)
            .HasForeignKey(m => m.RideId);

        builder.HasMany(r => r.RideTrafficStats)
            .WithOne(t => t.Ride)
            .HasForeignKey(t => t.RideId);
    }
}

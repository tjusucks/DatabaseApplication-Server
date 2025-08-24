using DbApp.Domain.Entities.ResourceSystem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DbApp.Infrastructure.Configurations.ResourceSystem;

/// <summary>
/// Entity Framework configuration for the AmusementRide entity.
/// Configures the amusement_rides table structure and relationships.
/// </summary>
public class AmusementRideConfiguration : IEntityTypeConfiguration<AmusementRide>
{
    public void Configure(EntityTypeBuilder<AmusementRide> builder)
    {
        // 表名和基础配置
        builder.ToTable("amusement_rides");

        // Primary key.
        builder.HasKey(r => r.RideId);

        builder.Property(r => r.RideId)
            .HasColumnName("ride_id")
            .HasColumnType("NUMBER(10)")
            .ValueGeneratedOnAdd();

        // Ride name - required.
        builder.Property(r => r.RideName)
            .HasColumnName("ride_name")
            .HasColumnType("VARCHAR2(100 CHAR)")
            .IsRequired();

        // Manager ID - foreign key.
        builder.Property(r => r.ManagerId)
            .HasColumnName("manager_id")
            .HasColumnType("NUMBER(10)");

        // Location - required.
        builder.Property(r => r.Location)
            .HasColumnName("location")
            .HasColumnType("VARCHAR2(100 CHAR)")
            .IsRequired();

        // Description - optional.
        builder.Property(r => r.Description)
            .HasColumnName("description")
            .HasColumnType("VARCHAR2(4000 CHAR)");

        // Ride status.
        builder.Property(r => r.RideStatus)
            .HasColumnName("ride_status")
            .IsRequired();

        // Capacity.
        builder.Property(r => r.Capacity)
            .HasColumnName("capacity")
            .HasColumnType("NUMBER(10)");

        // Duration in minutes.
        builder.Property(r => r.Duration)
            .HasColumnName("duration")
            .HasColumnType("NUMBER(10)");

        // Height limits.
        builder.Property(r => r.HeightLimitMin)
            .HasColumnName("height_limit_min")
            .HasColumnType("NUMBER(10)");

        builder.Property(r => r.HeightLimitMax)
            .HasColumnName("height_limit_max")
            .HasColumnType("NUMBER(10)");

        // Open date.
        builder.Property(r => r.OpenDate)
            .HasColumnName("open_date")
            .HasColumnType("TIMESTAMP(0)");

        // Audit fields.
        builder.Property(r => r.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("TIMESTAMP(0)")
            .IsRequired()
            .HasDefaultValueSql("SYSTIMESTAMP");

        builder.Property(r => r.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("TIMESTAMP(0)")
            .HasDefaultValueSql("SYSTIMESTAMP");

        // 索引配置
        builder.HasIndex(r => r.ManagerId);
        builder.HasIndex(r => r.RideName);
        builder.HasIndex(r => r.RideStatus);

        // 关系配置
        builder.HasOne(r => r.Manager)
            .WithMany(e => e.AmusementRides)
            .HasForeignKey(r => r.ManagerId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(r => r.InspectionRecords)
            .WithOne(i => i.Ride)
            .HasForeignKey(i => i.RideId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(r => r.MaintenanceRecords)
            .WithOne(m => m.Ride)
            .HasForeignKey(m => m.RideId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(r => r.RideTrafficStats)
            .WithOne(t => t.Ride)
            .HasForeignKey(t => t.RideId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

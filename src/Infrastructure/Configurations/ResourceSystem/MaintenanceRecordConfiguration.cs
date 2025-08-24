using DbApp.Domain.Entities.ResourceSystem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DbApp.Infrastructure.Configurations.ResourceSystem;

public class MaintenanceRecordConfiguration : IEntityTypeConfiguration<MaintenanceRecord>
{
    public void Configure(EntityTypeBuilder<MaintenanceRecord> builder)
    {
        // 表名和基础配置
        builder.ToTable("maintenance_records");
        builder.HasKey(r => r.MaintenanceId);

        // 属性映射
        builder.Property(r => r.MaintenanceId)
            .HasColumnName("maintenance_id")
            .HasColumnType("NUMBER(10)");

        builder.Property(r => r.RideId)
            .HasColumnName("ride_id")
            .HasColumnType("NUMBER(10)");

        builder.Property(r => r.TeamId)
            .HasColumnName("team_id")
            .HasColumnType("NUMBER(10)");

        builder.Property(r => r.ManagerId)
            .HasColumnName("manager_id")
            .HasColumnType("NUMBER(10)");

        builder.Property(r => r.MaintenanceType)
            .IsRequired()
            .HasColumnName("maintenance_type");

        builder.Property(r => r.StartTime)
            .HasColumnName("start_time")
            .HasColumnType("TIMESTAMP(0)");

        builder.Property(r => r.EndTime)
            .HasColumnName("end_time")
            .HasColumnType("TIMESTAMP(0)");

        builder.Property(r => r.Cost)
            .HasColumnName("cost")
            .HasColumnType("NUMBER(12,2)");

        builder.Property(r => r.PartsReplaced)
            .HasColumnName("parts_replaced")
            .HasColumnType("VARCHAR2(4000 CHAR)");

        builder.Property(r => r.MaintenanceDetails)
            .HasColumnName("maintenance_details")
            .HasColumnType("VARCHAR2(4000 CHAR)");

        builder.Property(r => r.IsCompleted)
            .IsRequired()
            .HasColumnName("is_completed")
            .HasColumnType("NUMBER(1)");

        builder.Property(r => r.IsAccepted)
            .HasColumnName("is_accepted")
            .HasColumnType("NUMBER(1)");

        builder.Property(r => r.AcceptanceDate)
            .HasColumnName("acceptance_date")
            .HasColumnType("TIMESTAMP(0)");

        builder.Property(r => r.AcceptanceComments)
            .HasColumnName("acceptance_comments")
            .HasColumnType("VARCHAR2(1000)");

        builder.Property(r => r.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("TIMESTAMP(0)")
            .HasDefaultValueSql("SYSTIMESTAMP");

        builder.Property(r => r.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("TIMESTAMP(0)")
            .HasDefaultValueSql("SYSTIMESTAMP");

        // 索引配置
        builder.HasIndex(r => r.EndTime);
        builder.HasIndex(r => r.TeamId);
        builder.HasIndex(r => r.IsAccepted);
        builder.HasIndex(r => r.IsCompleted);
        builder.HasIndex(r => r.ManagerId);
        builder.HasIndex(r => r.RideId);
        builder.HasIndex(r => r.StartTime);

        // 关系配置
        // 与设施的关系
        builder.HasOne(r => r.Ride)
            .WithMany(a => a.MaintenanceRecords)
            .HasForeignKey(r => r.RideId)
            .IsRequired();

        // 与维修组的关系
        builder.HasOne(r => r.Team)
            .WithMany(t => t.MaintenanceRecords)
            .HasForeignKey(r => r.TeamId)
            .IsRequired();

        // 与管理员的关系
        builder.HasOne(r => r.Manager)
            .WithMany()
            .HasForeignKey(r => r.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

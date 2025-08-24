using DbApp.Domain.Entities.ResourceSystem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DbApp.Infrastructure.Configurations.ResourceSystem;

public class MaintenanceRecordConfiguration : IEntityTypeConfiguration<MaintenanceRecord>
{
    public void Configure(EntityTypeBuilder<MaintenanceRecord> builder)
    {
        // 表名和基础配置
        builder.ToTable("MAINTENANCE_RECORDS");
        builder.HasKey(r => r.MaintenanceId);

        // 属性映射
        builder.Property(r => r.MaintenanceId)
            .HasColumnName("MAINTENANCE_ID")
            .HasPrecision(10);

        builder.Property(r => r.RideId)
            .HasColumnName("RIDE_ID")
            .HasPrecision(10);

        builder.Property(r => r.GroupId)
            .HasColumnName("GROUP_ID")
            .HasPrecision(10);

        builder.Property(r => r.ManagerId)
            .HasColumnName("MANAGER_ID")
            .HasPrecision(10);

        builder.Property(r => r.MaintenanceType)
            .IsRequired()
            .HasColumnName("MAINTENANCE_TYPE")
            .HasMaxLength(30)
            .IsUnicode(false)
            .HasConversion<string>();

        builder.Property(r => r.StartTime)
            .HasColumnName("START_TIME");

        builder.Property(r => r.EndTime)
            .HasColumnName("END_TIME");

        builder.Property(r => r.Cost)
            .HasColumnName("COST")
            .HasColumnType("decimal(12,2)"); // 12位精度，2位小数

        builder.Property(r => r.PartsReplaced)
            .HasColumnName("PARTS_REPLACED")
            .IsUnicode(false);

        builder.Property(r => r.MaintenanceDetails)
            .HasColumnName("MAINTENANCE_DETAILS")
            .IsUnicode(false);

        builder.Property(r => r.IsCompleted)
            .IsRequired()
            .HasColumnName("IS_COMPLETED")
            .HasColumnType("NUMBER(1)");

        builder.Property(r => r.IsAccepted)
            .HasColumnName("IS_ACCEPTED")
            .HasColumnType("NUMBER(1)");

        builder.Property(r => r.AcceptanceDate)
            .HasColumnName("ACCEPTANCE_DATE");

        builder.Property(r => r.AcceptanceComments)
            .HasColumnName("ACCEPTANCE_COMMENTS")
            .HasMaxLength(1000)
            .IsUnicode(false);

        builder.Property(r => r.CreatedAt)
            .HasColumnName("CREATED_AT");

        builder.Property(r => r.UpdatedAt)
            .HasColumnName("UPDATED_AT");

        // 索引配置（7个索引）
        builder.HasIndex(r => r.EndTime, "MAINTENANCE_RECORDS_END_TIME_IDX");
        builder.HasIndex(r => r.GroupId, "MAINTENANCE_RECORDS_GROUP_ID_IDX");
        builder.HasIndex(r => r.IsAccepted, "MAINTENANCE_RECORDS_IS_ACCEPTED_IDX");
        builder.HasIndex(r => r.IsCompleted, "MAINTENANCE_RECORDS_IS_COMPLETED_IDX");
        builder.HasIndex(r => r.ManagerId, "MAINTENANCE_RECORDS_MANAGER_ID_IDX");
        builder.HasIndex(r => r.RideId, "MAINTENANCE_RECORDS_RIDE_ID_IDX");
        builder.HasIndex(r => r.StartTime, "MAINTENANCE_RECORDS_START_TIME_IDX");

        // 关系配置
        // 与设施的关系
        builder.HasOne(r => r.Ride)
            .WithMany(a => a.MaintenanceRecords)
            .HasForeignKey(r => r.RideId)
            .IsRequired();

        // 与维修组的关系
        //builder.HasOne(r => r.Group)
        //.WithMany(t => t.MaintenanceRecords)
        //.HasForeignKey(r => r.GroupId)
        //.IsRequired();

        // 与管理员的关系
        //builder.HasOne(r => r.Manager)
        //.WithMany(e => e.MaintenanceRecords)
        //.HasForeignKey(r => r.ManagerId);
    }
}

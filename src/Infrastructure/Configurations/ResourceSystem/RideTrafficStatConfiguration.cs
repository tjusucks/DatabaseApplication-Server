using DbApp.Domain.Entities.ResourceSystem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DbApp.Infrastructure.Configurations.ResourceSystem;

public class RideTrafficStatConfiguration : IEntityTypeConfiguration<RideTrafficStat>
{
    public void Configure(EntityTypeBuilder<RideTrafficStat> builder)
    {
        // 表名和基础配置
        builder.ToTable("ride_traffic_stats");

        // 复合主键配置
        builder.HasKey(r => new { r.RideId, r.RecordTime });

        // 属性映射
        builder.Property(r => r.RideId)
            .HasColumnName("ride_id")
            .HasColumnType("NUMBER(10)");

        builder.Property(r => r.RecordTime)
            .HasColumnName("record_time")
            .HasColumnType("TIMESTAMP(0)");

        builder.Property(r => r.VisitorCount)
            .HasColumnName("visitor_count")
            .HasColumnType("NUMBER(10)")
            .HasDefaultValue(0);

        builder.Property(r => r.QueueLength)
            .HasColumnName("queue_length")
            .HasColumnType("NUMBER(10)")
            .HasDefaultValue(0);

        builder.Property(r => r.WaitingTime)
            .HasColumnName("waiting_time")
            .HasColumnType("NUMBER(10)")
            .HasDefaultValue(0);

        builder.Property(r => r.IsCrowded)
            .HasColumnName("is_crowded")
            .HasColumnType("NUMBER(1)")
            .HasDefaultValue(false);

        builder.Property(r => r.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("TIMESTAMP(0)")
            .HasDefaultValueSql("SYSTIMESTAMP");

        builder.Property(r => r.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("TIMESTAMP(0)")
            .HasDefaultValueSql("SYSTIMESTAMP");

        // 索引配置
        builder.HasIndex(r => r.IsCrowded);

        // 关系配置
        builder.HasOne(r => r.Ride)
            .WithMany(a => a.RideTrafficStats)
            .HasForeignKey(r => r.RideId)
            .IsRequired();
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DbApp.Domain.Entities;

namespace DbApp.Infrastructure.Configurations;

public class RideTrafficStatConfiguration : IEntityTypeConfiguration<RideTrafficStat>
{
    public void Configure(EntityTypeBuilder<RideTrafficStat> builder)
    {
        // 表名和基础配置
        builder.ToTable("RIDE_TRAFFIC_STATS");
        
        // 复合主键配置
        builder.HasKey(r => new { r.RideId, r.RecordTime });

        // 属性映射
        builder.Property(r => r.RideId)
            .HasColumnName("RIDE_ID")
            .HasPrecision(10);

        builder.Property(r => r.RecordTime)
            .HasColumnName("RECORD_TIME");

        builder.Property(r => r.VisitorCount)
            .HasColumnName("VISITOR_COUNT")
            .HasPrecision(10);

        builder.Property(r => r.QueueLength)
            .HasColumnName("QUEUE_LENGTH")
            .HasPrecision(10);

        builder.Property(r => r.WaitingTime)
            .HasColumnName("WAITING_TIME")
            .HasPrecision(10);

        builder.Property(r => r.IsCrowded)
            .HasColumnName("IS_CROWDED")
            .HasColumnType("NUMBER(1)");

        builder.Property(r => r.CreatedAt)
            .HasColumnName("CREATED_AT");

        builder.Property(r => r.UpdatedAt)
            .HasColumnName("UPDATED_AT");

        // 索引配置
        builder.HasIndex(r => r.IsCrowded, "RIDE_TRAFFIC_STATS_IS_CROWDED_IDX");

        // 关系配置
        builder.HasOne(r => r.Ride)
            .WithMany(a => a.RideTrafficStats)
            .HasForeignKey(r => r.RideId)
            .IsRequired();
    }
}
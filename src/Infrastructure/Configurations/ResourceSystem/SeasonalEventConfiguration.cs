using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Enums.ResourceSystem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DbApp.Infrastructure.Configurations.ResourceSystem;

public class SeasonalEventConfiguration : IEntityTypeConfiguration<SeasonalEvent>
{
    public void Configure(EntityTypeBuilder<SeasonalEvent> builder)
    {
        // 表名和基础配置
        builder.ToTable("seasonal_events");
        builder.HasKey(e => e.EventId);

        // 属性映射
        builder.Property(e => e.EventId)
            .HasColumnName("event_id")
            .HasColumnType("NUMBER(10)");

        builder.Property(e => e.EventName)
            .IsRequired()
            .HasColumnName("event_name")
            .HasColumnType("VARCHAR2(100)");

        builder.Property(e => e.EventType)
            .IsRequired()
            .HasColumnName("event_type");

        builder.Property(e => e.Description)
            .HasColumnName("description")
            .HasColumnType("VARCHAR2(4000 CHAR)");

        builder.Property(e => e.StartDate)
            .HasColumnName("start_date")
            .HasColumnType("TIMESTAMP(0)");

        builder.Property(e => e.EndDate)
            .HasColumnName("end_date")
            .HasColumnType("TIMESTAMP(0)");

        builder.Property(e => e.Location)
            .HasColumnName("location")
            .HasColumnType("VARCHAR2(100)");

        builder.Property(e => e.Budget)
            .HasColumnName("budget")
            .HasColumnType("NUMBER(12,2)");

        builder.Property(e => e.MaxCapacity)
            .HasColumnName("max_capacity")
            .HasColumnType("NUMBER(10)");

        builder.Property(e => e.TicketPrice)
            .HasColumnName("ticket_price")
            .HasColumnType("NUMBER(8,2)");

        builder.Property(e => e.Status)
            .IsRequired()
            .HasColumnName("status")
            .HasDefaultValue(EventStatus.Planning);

        builder.Property(e => e.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("TIMESTAMP(0)")
            .HasDefaultValueSql("SYSTIMESTAMP");

        builder.Property(e => e.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("TIMESTAMP(0)")
            .HasDefaultValueSql("SYSTIMESTAMP");

        // 索引配置
        builder.HasIndex(e => e.EndDate);
        builder.HasIndex(e => e.EventName);
        builder.HasIndex(e => e.EventType);
        builder.HasIndex(e => e.StartDate);
        builder.HasIndex(e => e.Status);
    }
}

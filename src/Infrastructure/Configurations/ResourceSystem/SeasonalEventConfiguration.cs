using DbApp.Domain.Entities.ResourceSystem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DbApp.Infrastructure.Configurations.ResourceSystem;

public class SeasonalEventConfiguration : IEntityTypeConfiguration<SeasonalEvent>
{
    public void Configure(EntityTypeBuilder<SeasonalEvent> builder)
    {
        // 表名和基础配置
        builder.ToTable("SEASONAL_EVENTS");
        builder.HasKey(e => e.EventId);

        // 属性映射
        builder.Property(e => e.EventId)
            .HasColumnName("EVENT_ID")
            .HasPrecision(10);

        builder.Property(e => e.EventName)
            .IsRequired()
            .HasColumnName("EVENT_NAME")
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.Property(e => e.EventType)
            .IsRequired()
            .HasColumnName("EVENT_TYPE")
            .HasMaxLength(30)
            .IsUnicode(false)
            .HasConversion<string>();

        builder.Property(e => e.Description)
            .HasColumnName("DESCRIPTION")
            .IsUnicode(false);

        builder.Property(e => e.StartDate)
            .HasColumnName("START_DATE");

        builder.Property(e => e.EndDate)
            .HasColumnName("END_DATE");

        builder.Property(e => e.Location)
            .HasColumnName("LOCATION")
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.Property(e => e.Budget)
            .HasColumnName("BUDGET")
            .HasColumnType("decimal(12,2)"); // 12位精度，2位小数

        builder.Property(e => e.MaxCapacity)
            .HasColumnName("MAX_CAPACITY")
            .HasPrecision(10);

        builder.Property(e => e.TicketPrice)
            .HasColumnName("TICKET_PRICE")
            .HasColumnType("decimal(8,2)"); // 8位精度，2位小数

        builder.Property(e => e.Status)
            .IsRequired()
            .HasColumnName("STATUS")
            .HasMaxLength(30)
            .IsUnicode(false);

        builder.Property(e => e.CreatedAt)
            .HasColumnName("CREATED_AT");

        builder.Property(e => e.UpdatedAt)
            .HasColumnName("UPDATED_AT");

        // 索引配置（5个索引）
        builder.HasIndex(e => e.EndDate, "SEASONAL_EVENTS_END_DATE_IDX");
        builder.HasIndex(e => e.EventName, "SEASONAL_EVENTS_EVENT_NAME_IDX");
        builder.HasIndex(e => e.EventType, "SEASONAL_EVENTS_EVENT_TYPE_IDX");
        builder.HasIndex(e => e.StartDate, "SEASONAL_EVENTS_START_DATE_IDX");
        builder.HasIndex(e => e.Status, "SEASONAL_EVENTS_STATUS_IDX");
    }
}

using DbApp.Domain.Entities.TicketingSystem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DbApp.Infrastructure.Configurations.TicketingSystem;

public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.ToTable("tickets");

        builder.HasKey(t => t.TicketId);

        builder.Property(t => t.TicketId)
            .HasColumnName("ticket_id")
            .HasColumnType("NUMBER(10)");

        builder.Property(t => t.ReservationItemId)
            .HasColumnName("reservation_item_id")
            .HasColumnType("NUMBER(10)")
            .IsRequired();

        builder.Property(t => t.TicketTypeId)
            .HasColumnName("ticket_type_id")
            .HasColumnType("NUMBER(10)")
            .IsRequired();

        builder.Property(t => t.VisitorId)
            .HasColumnName("visitor_id")
            .HasColumnType("NUMBER(10)");

        builder.Property(t => t.SerialNumber)
            .HasColumnName("serial_number")
            .HasColumnType("VARCHAR2(50 CHAR)")
            .IsRequired();

        // 为 SerialNumber 创建唯一索引
        builder.HasIndex(t => t.SerialNumber)
            .IsUnique();

        builder.Property(t => t.ValidFrom)
            .HasColumnName("valid_from")
            .HasColumnType("TIMESTAMP")
            .IsRequired();

        builder.Property(t => t.ValidTo)
            .HasColumnName("valid_to")
            .HasColumnType("TIMESTAMP")
            .IsRequired();

        builder.Property(t => t.Status)
            .HasColumnName("status")
            .IsRequired();

        builder.Property(t => t.UsedTime)
            .HasColumnName("used_time")
            .HasColumnType("TIMESTAMP");

        builder.Property(t => t.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("TIMESTAMP")
            .IsRequired()
            .HasDefaultValueSql("SYSTIMESTAMP");

        builder.Property(t => t.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("TIMESTAMP")
            .IsRequired()
            .HasDefaultValueSql("SYSTIMESTAMP"); // 同样，这只对创建时有效，更新需要额外逻辑

        // 配置外键关系
        builder.HasOne(t => t.ReservationItem)
            .WithMany()
            .HasForeignKey(t => t.ReservationItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.TicketType)
            .WithMany()
            .HasForeignKey(t => t.TicketTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Visitor)
            .WithMany()
            .HasForeignKey(t => t.VisitorId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

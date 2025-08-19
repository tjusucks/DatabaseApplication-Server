using DbApp.Domain.Entities; 
using DbApp.Domain.Enums;    
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DbApp.Domain.Entities.TicketRelated;



namespace DbApp.Infrastructure.Configurations.TicketRelated
{
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
            builder.HasIndex(t => t.SerialNumber, "UQ_tickets_serial_number")
                .IsUnique();

            builder.Property(t => t.ValidFrom)
                .HasColumnName("valid_from")
                .HasColumnType("TIMESTAMP(0)")
                .IsRequired();

            builder.Property(t => t.ValidTo)
                .HasColumnName("valid_to")
                .HasColumnType("TIMESTAMP(0)")
                .IsRequired();

            builder.Property(t => t.Status)
                .HasColumnName("status")
                .HasColumnType("VARCHAR2(30 CHAR)")
                .IsRequired()
                .HasConversion(
                    v => v.ToString().ToLower(),
                    v => (TicketStatus)Enum.Parse(typeof(TicketStatus), v, true));

            // 添加 CHECK 约束
            builder.HasCheckConstraint("CK_tickets_status", "status IN ('issued', 'used', 'expired', 'refunded', 'cancelled')");

            builder.Property(t => t.UsedTime)
                .HasColumnName("used_time")
                .HasColumnType("TIMESTAMP(0)");

            builder.Property(t => t.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("TIMESTAMP(0)")
                .IsRequired()
                .HasDefaultValueSql("SYSTIMESTAMP");

            builder.Property(t => t.UpdatedAt)
                .HasColumnName("updated_at")
                .HasColumnType("TIMESTAMP(0)")
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
}
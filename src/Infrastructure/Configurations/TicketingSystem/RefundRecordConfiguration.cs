using DbApp.Domain.Entities.TicketingSystem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DbApp.Infrastructure.Configurations.TicketingSystem;

public class RefundRecordConfiguration : IEntityTypeConfiguration<RefundRecord>
{
    public void Configure(EntityTypeBuilder<RefundRecord> builder)
    {
        builder.ToTable("refund_records");

        builder.HasKey(rr => rr.RefundId);

        builder.Property(rr => rr.RefundId).HasColumnName("refund_id").HasColumnType("NUMBER(10)");
        builder.Property(rr => rr.TicketId).HasColumnName("ticket_id").HasColumnType("NUMBER(10)").IsRequired();
        builder.Property(rr => rr.VisitorId).HasColumnName("visitor_id").HasColumnType("NUMBER(10)").IsRequired();

        builder.Property(rr => rr.RefundAmount).HasColumnName("refund_amount").HasColumnType("NUMBER(10,2)").IsRequired();

        builder.Property(rr => rr.RefundTime).HasColumnName("refund_time").HasColumnType("TIMESTAMP(0)").IsRequired().HasDefaultValueSql("SYSTIMESTAMP");
        builder.Property(rr => rr.RefundReason).HasColumnName("refund_reason").HasColumnType("VARCHAR2(500 CHAR)");

        // 配置 RefundStatus 枚举
        builder.Property(rr => rr.RefundStatus)
            .HasColumnName("refund_status")
            .IsRequired();

        builder.Property(rr => rr.ProcessorId).HasColumnName("processor_id").HasColumnType("NUMBER(10)");
        builder.Property(rr => rr.ProcessingNotes).HasColumnName("processing_notes").HasColumnType("VARCHAR2(500 CHAR)");

        builder.Property(rr => rr.CreatedAt).HasColumnName("created_at").HasColumnType("TIMESTAMP(0)").IsRequired().HasDefaultValueSql("SYSTIMESTAMP");
        builder.Property(rr => rr.UpdatedAt).HasColumnName("updated_at").HasColumnType("TIMESTAMP(0)").IsRequired().HasDefaultValueSql("SYSTIMESTAMP");

        // 配置索引
        builder.HasIndex(rr => rr.TicketId).IsUnique(); // 确保一张票只有一个退款记录
        builder.HasIndex(rr => rr.VisitorId);
        builder.HasIndex(rr => rr.RefundTime);
        builder.HasIndex(rr => rr.RefundStatus);

        // 配置外键关系
        // 与 Ticket 的一对一关系
        builder.HasOne(rr => rr.Ticket)
               .WithOne(t => t.RefundRecord) // 对应 Ticket 实体中的 RefundRecord 属性
               .HasForeignKey<RefundRecord>(rr => rr.TicketId) // 外键在 RefundRecord 表中
               .OnDelete(DeleteBehavior.Cascade); // 如果票被删除，退款记录也应删除 (根据业务决定)

        builder.HasOne(rr => rr.Visitor)
               .WithMany()
               .HasForeignKey(rr => rr.VisitorId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(rr => rr.Processor)
               .WithMany()
               .HasForeignKey(rr => rr.ProcessorId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}


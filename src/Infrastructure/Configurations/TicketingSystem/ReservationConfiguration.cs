using DbApp.Domain.Entities.TicketingSystem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DbApp.Infrastructure.Configurations.TicketingSystem;

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable("reservations", r =>
        {
            r.HasCheckConstraint("CK_reservations_discount_amount_Range", "\"discount_amount\" >= 0");
            r.HasCheckConstraint("CK_reservations_total_amount_Range", "\"total_amount\" >= 0");
        });

        builder.HasKey(r => r.ReservationId);

        builder.Property(r => r.ReservationId).HasColumnName("reservation_id").HasColumnType("NUMBER(10)");
        builder.Property(r => r.VisitorId).HasColumnName("visitor_id").HasColumnType("NUMBER(10)").IsRequired();
        builder.Property(r => r.ReservationTime).HasColumnName("reservation_time").HasColumnType("TIMESTAMP").IsRequired().HasDefaultValueSql("SYSTIMESTAMP");
        builder.Property(r => r.VisitDate).HasColumnName("visit_date").HasColumnType("TIMESTAMP").IsRequired();

        builder.Property(r => r.DiscountAmount).HasColumnName("discount_amount").HasColumnType("NUMBER(10,2)").IsRequired().HasDefaultValue(0);

        builder.Property(r => r.TotalAmount).HasColumnName("total_amount").HasColumnType("NUMBER(10,2)").IsRequired();

        // 配置 PaymentStatus 枚举
        builder.Property(r => r.PaymentStatus)
            .HasColumnName("payment_status")
            .IsRequired();

        // 配置 ReservationStatus 枚举
        builder.Property(r => r.Status)
            .HasColumnName("status")
            .IsRequired();

        builder.Property(r => r.PaymentMethod).HasColumnName("payment_method").HasColumnType("VARCHAR2(30 CHAR)");
        builder.Property(r => r.PromotionId).HasColumnName("promotion_id").HasColumnType("NUMBER(10)");
        builder.Property(r => r.SpecialRequests).HasColumnName("special_requests").HasColumnType("VARCHAR2(500 CHAR)");

        builder.Property(r => r.CreatedAt).HasColumnName("created_at").HasColumnType("TIMESTAMP").IsRequired().HasDefaultValueSql("SYSTIMESTAMP");
        builder.Property(r => r.UpdatedAt).HasColumnName("updated_at").HasColumnType("TIMESTAMP").IsRequired().HasDefaultValueSql("SYSTIMESTAMP");

        // 配置索引
        builder.HasIndex(r => r.VisitorId);
        builder.HasIndex(r => r.ReservationTime);
        builder.HasIndex(r => r.VisitDate);
        builder.HasIndex(r => r.PaymentStatus);
        builder.HasIndex(r => r.Status);

        // 配置外键
        builder.HasOne(r => r.Visitor)
               .WithMany(v => v.Reservations)
               .HasForeignKey(r => r.VisitorId)
               .OnDelete(DeleteBehavior.Restrict); // 禁止删除有预订记录的游客

        builder.HasOne(r => r.Promotion)
               .WithMany(p => p.Reservations)
               .HasForeignKey(r => r.PromotionId)
               .OnDelete(DeleteBehavior.SetNull); // 如果优惠活动被删除，将预订中的ID设为null
    }
}

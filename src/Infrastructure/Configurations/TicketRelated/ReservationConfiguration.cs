using DbApp.Domain.Entities.TicketRelated;
using DbApp.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace DbApp.Infrastructure.Configurations.TicketRelated
{
public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable("reservations");

        builder.HasKey(r => r.ReservationId);

        builder.Property(r => r.ReservationId).HasColumnName("reservation_id").HasColumnType("NUMBER(10)");
        builder.Property(r => r.VisitorId).HasColumnName("visitor_id").HasColumnType("NUMBER(10)").IsRequired();
        builder.Property(r => r.ReservationTime).HasColumnName("reservation_time").HasColumnType("TIMESTAMP(0)").IsRequired().HasDefaultValueSql("SYSTIMESTAMP");
        builder.Property(r => r.VisitDate).HasColumnName("visit_date").HasColumnType("TIMESTAMP(0)").IsRequired();
        
        builder.Property(r => r.DiscountAmount).HasColumnName("discount_amount").HasColumnType("NUMBER(10,2)").IsRequired().HasDefaultValue(0);
        
        builder.Property(r => r.TotalAmount).HasColumnName("total_amount").HasColumnType("NUMBER(10,2)").IsRequired();
        builder.HasCheckConstraint("CK_reservations_total_amount", "total_amount >= 0");

        // 配置 PaymentStatus 枚举
        builder.Property(r => r.PaymentStatus)
            .HasColumnName("payment_status")
            .HasColumnType("VARCHAR2(30 CHAR)")
            .IsRequired()
            .HasConversion(v => v.ToString().ToLower(), v => (PaymentStatus)Enum.Parse(typeof(PaymentStatus), v, true));
        builder.HasCheckConstraint("CK_reservations_payment_status", "payment_status IN ('pending', 'paid', 'failed', 'refunded')");

        // 配置 ReservationStatus 枚举
        builder.Property(r => r.Status)
            .HasColumnName("status")
            .HasColumnType("VARCHAR2(30 CHAR)")
            .IsRequired()
            .HasConversion(v => v.ToString().ToLower(), v => (ReservationStatus)Enum.Parse(typeof(ReservationStatus), v, true));
        builder.HasCheckConstraint("CK_reservations_status", "status IN ('pending', 'confirmed', 'cancelled', 'completed')");

        builder.Property(r => r.PaymentMethod).HasColumnName("payment_method").HasColumnType("VARCHAR2(30 CHAR)");
        builder.Property(r => r.PromotionId).HasColumnName("promotion_id").HasColumnType("NUMBER(10)");
        builder.Property(r => r.SpecialRequests).HasColumnName("special_requests").HasColumnType("VARCHAR2(500 CHAR)");

        builder.Property(r => r.CreatedAt).HasColumnName("created_at").HasColumnType("TIMESTAMP(0)").IsRequired().HasDefaultValueSql("SYSTIMESTAMP");
        builder.Property(r => r.UpdatedAt).HasColumnName("updated_at").HasColumnType("TIMESTAMP(0)").IsRequired().HasDefaultValueSql("SYSTIMESTAMP");
        
        // 配置索引
        builder.HasIndex(r => r.VisitorId);
        builder.HasIndex(r => r.ReservationTime);
        builder.HasIndex(r => r.VisitDate);
        builder.HasIndex(r => r.PaymentStatus);
        builder.HasIndex(r => r.Status);

        // 配置外键
        builder.HasOne(r => r.Visitor)
               .WithMany() // 假设 Visitor 不需要 Reservations 集合
               .HasForeignKey(r => r.VisitorId)
               .OnDelete(DeleteBehavior.Restrict); // 禁止删除有预订记录的游客

        builder.HasOne(r => r.Promotion)
               .WithMany() // 假设 Promotion 不需要 Reservations 集合
               .HasForeignKey(r => r.PromotionId)
               .OnDelete(DeleteBehavior.SetNull); // 如果优惠活动被删除，将预订中的ID设为null
    }
}
}
using DbApp.Domain.Entities.TicketingSystem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DbApp.Infrastructure.Configurations.TicketingSystem;

public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
{
    public void Configure(EntityTypeBuilder<Coupon> builder)
    {
        builder.ToTable("coupons");

        builder.HasKey(c => c.CouponId);

        builder.Property(c => c.CouponId).HasColumnName("coupon_id").HasColumnType("NUMBER(10)");

        builder.Property(c => c.CouponCode).HasColumnName("coupon_code").HasColumnType("VARCHAR2(50 CHAR)").IsRequired();
        builder.HasIndex(c => c.CouponCode).IsUnique();

        builder.Property(c => c.PromotionId).HasColumnName("promotion_id").HasColumnType("NUMBER(10)").IsRequired();

        builder.Property(c => c.DiscountType)
            .HasColumnName("discount_type")
            .IsRequired();

        builder.Property(c => c.DiscountValue).HasColumnName("discount_value").HasColumnType("NUMBER(10,2)").IsRequired();
        builder.Property(c => c.MinPurchaseAmount).HasColumnName("min_purchase_amount").HasColumnType("NUMBER(10,2)").IsRequired().HasDefaultValue(0);
        builder.Property(c => c.ValidFrom).HasColumnName("valid_from").HasColumnType("TIMESTAMP").IsRequired();
        builder.Property(c => c.ValidTo).HasColumnName("valid_to").HasColumnType("TIMESTAMP").IsRequired();

        builder.Property(c => c.IsUsed)
            .HasColumnName("is_used")
            .HasColumnType("NUMBER(1)")
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(c => c.UsedById).HasColumnName("used_by").HasColumnType("NUMBER(10)");
        builder.Property(c => c.UsedTime).HasColumnName("used_time").HasColumnType("TIMESTAMP");

        builder.Property(c => c.CreatedAt).HasColumnName("created_at").HasColumnType("TIMESTAMP").IsRequired().HasDefaultValueSql("SYSTIMESTAMP");
        builder.Property(c => c.UpdatedAt).HasColumnName("updated_at").HasColumnType("TIMESTAMP").IsRequired().HasDefaultValueSql("SYSTIMESTAMP");

        // 配置索引
        builder.HasIndex(c => c.PromotionId);
        builder.HasIndex(c => c.ValidFrom);
        builder.HasIndex(c => c.ValidTo);
        builder.HasIndex(c => c.IsUsed);

        // 配置外键
        builder.HasOne(c => c.Promotion)
               .WithMany(p => p.Coupons)
               .HasForeignKey(c => c.PromotionId)
               .OnDelete(DeleteBehavior.Cascade); // 强依赖关系，活动删除则券也失效

        builder.HasOne(c => c.UsedBy)
               .WithMany(v => v.UsedCoupons) // 假设 Visitor 有 UsedCoupons 集合
               .HasForeignKey(c => c.UsedById)
               .OnDelete(DeleteBehavior.SetNull); // 游客注销，保留用券记录但清空关联
    }
}

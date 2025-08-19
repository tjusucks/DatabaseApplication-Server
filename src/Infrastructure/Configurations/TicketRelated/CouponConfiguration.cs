using DbApp.Domain.Entities.TicketRelated;
using DbApp.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
namespace DbApp.Infrastructure.Configurations.TicketRelated
{
    public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
    {
        public void Configure(EntityTypeBuilder<Coupon> builder)
        {
            builder.ToTable("coupons");

            builder.HasKey(c => c.CouponId);

            builder.Property(c => c.CouponId).HasColumnName("coupon_id").HasColumnType("NUMBER(10)");

            builder.Property(c => c.CouponCode).HasColumnName("coupon_code").HasColumnType("VARCHAR2(50 CHAR)").IsRequired();
            builder.HasIndex(c => c.CouponCode).IsUnique(); // 唯一约束

            builder.Property(c => c.PromotionId).HasColumnName("promotion_id").HasColumnType("NUMBER(10)").IsRequired();

            // 配置 DiscountType 枚举
            builder.Property(c => c.DiscountType)
                .HasColumnName("discount_type")
                .HasColumnType("VARCHAR2(30 CHAR)")
                .IsRequired()
                .HasConversion(new EnumToStringConverter<CouponDiscountType>()); // 自动转为 "Percentage", "FixedAmount"
            builder.HasCheckConstraint("CK_coupons_discount_type", "discount_type IN ('Percentage', 'FixedAmount')");

            builder.Property(c => c.DiscountValue).HasColumnName("discount_value").HasColumnType("NUMBER(10,2)").IsRequired();
            builder.Property(c => c.MinPurchaseAmount).HasColumnName("min_purchase_amount").HasColumnType("NUMBER(10,2)").IsRequired().HasDefaultValue(0);
            builder.Property(c => c.ValidFrom).HasColumnName("valid_from").HasColumnType("TIMESTAMP(0)").IsRequired();
            builder.Property(c => c.ValidTo).HasColumnName("valid_to").HasColumnType("TIMESTAMP(0)").IsRequired();

            // 配置 bool 到 NUMBER(1) 的转换
            builder.Property(c => c.IsUsed)
                .HasColumnName("is_used")
                .HasColumnType("NUMBER(1)")
                .IsRequired()
                .HasDefaultValue(false)
                .HasConversion(new BoolToZeroOneConverter<short>());

            builder.Property(c => c.UsedById).HasColumnName("used_by").HasColumnType("NUMBER(10)");
            builder.Property(c => c.UsedTime).HasColumnName("used_time").HasColumnType("TIMESTAMP(0)");

            builder.Property(c => c.CreatedAt).HasColumnName("created_at").HasColumnType("TIMESTAMP(0)").IsRequired().HasDefaultValueSql("SYSTIMESTAMP");
            builder.Property(c => c.UpdatedAt).HasColumnName("updated_at").HasColumnType("TIMESTAMP(0)").IsRequired().HasDefaultValueSql("SYSTIMESTAMP");

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
}
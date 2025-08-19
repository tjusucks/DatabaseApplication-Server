using DbApp.Infrastructure.Converters;
using DbApp.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DbApp.Domain.Entities.TicketRelated;

namespace DbApp.Infrastructure.Configurations.TicketRelated
{
    public class PromotionConfiguration : IEntityTypeConfiguration<Promotion>
    {
        public void Configure(EntityTypeBuilder<Promotion> builder)
        {
            builder.ToTable("promotions");

            builder.HasKey(p => p.PromotionId);

            builder.Property(p => p.PromotionId)
                .HasColumnName("promotion_id")
                .HasColumnType("NUMBER(10)");

            builder.Property(p => p.PromotionName)
                .HasColumnName("promotion_name")
                .HasColumnType("VARCHAR2(100 CHAR)")
                .IsRequired();

            builder.Property(p => p.PromotionType)
     .HasColumnName("promotion_type")
     .HasColumnType("VARCHAR2(30 CHAR)")
     .IsRequired()
     // 直接 new 一个转换器实例即可！
     .HasConversion(new PromotionTypeToStringConverter());

            // 配置 CHECK 约束
            builder.HasCheckConstraint("CK_promotions_promotion_type",
                "promotion_type IN ('DISCOUNT_PERCENT', 'DISCOUNT_FIXED', 'FULL_REDUCTION', 'FULL_GIFT', 'PACKAGE_DEAL', 'COUPON_BASED')");

            builder.Property(p => p.Description)
                .HasColumnName("description")
                .HasColumnType("VARCHAR2(4000 CHAR)");

            builder.Property(p => p.StartDatetime)
                .HasColumnName("start_datetime")
                .HasColumnType("TIMESTAMP(0)")
                .IsRequired();

            builder.Property(p => p.EndDatetime)
                .HasColumnName("end_datetime")
                .HasColumnType("TIMESTAMP(0)")
                .IsRequired();

            builder.Property(p => p.UsageLimitPerUser)
                .HasColumnName("usage_limit_per_user")
                .HasColumnType("NUMBER(5)");

            builder.Property(p => p.TotalUsageLimit)
                .HasColumnName("total_usage_limit")
                .HasColumnType("NUMBER(10)");

            // 配置带默认值的字段
            builder.Property(p => p.CurrentUsageCount)
                .HasColumnName("current_usage_count")
                .HasColumnType("NUMBER(10)")
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(p => p.DisplayPriority)
                .HasColumnName("display_priority")
                .HasColumnType("NUMBER(5)")
                .IsRequired()
                .HasDefaultValue(100);

            // 配置 bool 到 NUMBER(1) 的转换
            var boolToNumberConverter = new BoolToZeroOneConverter<short>();

            builder.Property(p => p.AppliesToAllTickets)
                .HasColumnName("applies_to_all_tickets")
                .HasColumnType("NUMBER(1)")
                .IsRequired()
                .HasDefaultValue(false)
                .HasConversion(boolToNumberConverter);

            builder.Property(p => p.IsActive)
                .HasColumnName("is_active")
                .HasColumnType("NUMBER(1)")
                .IsRequired()
                .HasDefaultValue(true)
                .HasConversion(boolToNumberConverter);

            builder.Property(p => p.IsCombinable)
                .HasColumnName("is_combinable")
                .HasColumnType("NUMBER(1)")
                .IsRequired()
                .HasDefaultValue(false)
                .HasConversion(boolToNumberConverter);

            builder.Property(p => p.EmployeeId)
                .HasColumnName("employee_id")
                .HasColumnType("NUMBER(10)")
                .IsRequired();

            builder.Property(p => p.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("TIMESTAMP(0)")
                .IsRequired()
                .HasDefaultValueSql("SYSTIMESTAMP");

            builder.Property(p => p.UpdatedAt)
                .HasColumnName("updated_at")
                .HasColumnType("TIMESTAMP(0)")
                .IsRequired()
                .HasDefaultValueSql("SYSTIMESTAMP");

            // 配置索引
            builder.HasIndex(p => p.PromotionName);
            builder.HasIndex(p => p.PromotionType);
            builder.HasIndex(p => p.StartDatetime);
            builder.HasIndex(p => p.EndDatetime);
            builder.HasIndex(p => p.DisplayPriority);
            builder.HasIndex(p => p.AppliesToAllTickets);
            builder.HasIndex(p => p.IsActive);

            // 配置外键
            builder.HasOne(p => p.Employee)
                   .WithMany()
                   .HasForeignKey(p => p.EmployeeId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
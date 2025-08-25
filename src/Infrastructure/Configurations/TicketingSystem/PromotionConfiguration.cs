using DbApp.Domain.Entities.TicketingSystem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DbApp.Infrastructure.Configurations.TicketingSystem;

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
            .IsRequired();

        builder.Property(p => p.Description)
            .HasColumnName("description")
            .HasColumnType("VARCHAR2(4000 CHAR)");

        builder.Property(p => p.StartDatetime)
            .HasColumnName("start_datetime")
            .HasColumnType("TIMESTAMP")
            .IsRequired();

        builder.Property(p => p.EndDatetime)
            .HasColumnName("end_datetime")
            .HasColumnType("TIMESTAMP")
            .IsRequired();

        builder.Property(p => p.UsageLimitPerUser)
            .HasColumnName("usage_limit_per_user")
            .HasColumnType("NUMBER(10)");

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
            .HasColumnType("NUMBER(10)")
            .IsRequired()
            .HasDefaultValue(100);

        builder.Property(p => p.AppliesToAllTickets)
            .HasColumnName("applies_to_all_tickets")
            .HasColumnType("NUMBER(1)")
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(p => p.IsActive)
            .HasColumnName("is_active")
            .HasColumnType("NUMBER(1)")
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(p => p.IsCombinable)
            .HasColumnName("is_combinable")
            .HasColumnType("NUMBER(1)")
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(p => p.EmployeeId)
            .HasColumnName("employee_id")
            .HasColumnType("NUMBER(10)")
            .IsRequired();

        builder.Property(p => p.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("TIMESTAMP")
            .IsRequired()
            .HasDefaultValueSql("SYSTIMESTAMP");

        builder.Property(p => p.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("TIMESTAMP")
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

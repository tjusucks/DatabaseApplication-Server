using DbApp.Domain.Entities.TicketingSystem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DbApp.Infrastructure.Configurations.TicketingSystem;

public class PromotionConditionConfiguration : IEntityTypeConfiguration<PromotionCondition>
{
    public void Configure(EntityTypeBuilder<PromotionCondition> builder)
    {
        builder.ToTable("promotion_conditions");

        builder.HasKey(pc => pc.ConditionId);

        builder.Property(pc => pc.ConditionId)
            .HasColumnName("condition_id")
            .HasColumnType("NUMBER(10)");

        builder.Property(pc => pc.PromotionId)
            .HasColumnName("promotion_id")
            .HasColumnType("NUMBER(10)")
            .IsRequired();

        builder.Property(pc => pc.ConditionName)
            .HasColumnName("condition_name")
            .HasColumnType("VARCHAR2(100 CHAR)")
            .IsRequired();

        // 配置 ConditionType 枚举
        builder.Property(pc => pc.ConditionType)
            .HasColumnName("condition_type")
            .IsRequired();

        builder.Property(pc => pc.TicketTypeId).HasColumnName("ticket_type_id").HasColumnType("NUMBER(10)");
        builder.Property(pc => pc.MinQuantity).HasColumnName("min_quantity").HasColumnType("NUMBER(5)");
        builder.Property(pc => pc.MinAmount).HasColumnName("min_amount").HasColumnType("NUMBER(10,2)");
        builder.Property(pc => pc.VisitorType).HasColumnName("visitor_type").HasColumnType("VARCHAR2(30 CHAR)");
        builder.Property(pc => pc.MemberLevel).HasColumnName("member_level").HasColumnType("VARCHAR2(30 CHAR)");
        builder.Property(pc => pc.DateFrom).HasColumnName("date_from").HasColumnType("TIMESTAMP(0)");
        builder.Property(pc => pc.DateTo).HasColumnName("date_to").HasColumnType("TIMESTAMP(0)");

        builder.Property(pc => pc.DayOfWeek)
            .HasColumnName("day_of_week")
            .HasColumnType("NUMBER(1)");

        builder.Property(pc => pc.Priority)
            .HasColumnName("priority")
            .HasColumnType("NUMBER(5)")
            .IsRequired()
            .HasDefaultValue(10);

        builder.Property(pc => pc.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("TIMESTAMP(0)")
            .IsRequired()
            .HasDefaultValueSql("SYSTIMESTAMP");

        builder.Property(pc => pc.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("TIMESTAMP(0)")
            .IsRequired()
            .HasDefaultValueSql("SYSTIMESTAMP");

        // 配置索引
        builder.HasIndex(pc => pc.PromotionId);
        builder.HasIndex(pc => pc.ConditionType);
        builder.HasIndex(pc => pc.TicketTypeId);
        builder.HasIndex(pc => pc.Priority);

        // 配置外键
        builder.HasOne(pc => pc.Promotion)
               .WithMany(p => p.Conditions)
               .HasForeignKey(pc => pc.PromotionId)
               .OnDelete(DeleteBehavior.Cascade); // 条件依赖于活动，活动删除则条件也删除

        builder.HasOne(pc => pc.TicketType)
               .WithMany()
               .HasForeignKey(pc => pc.TicketTypeId)
               .OnDelete(DeleteBehavior.SetNull); // 如果票种被删除，将条件中的票种ID设为null
    }
}


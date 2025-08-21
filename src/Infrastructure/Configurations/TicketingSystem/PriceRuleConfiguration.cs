
using DbApp.Domain.Entities.TicketingSystem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DbApp.Infrastructure.Configurations.TicketingSystem;

public class PriceRuleConfiguration : IEntityTypeConfiguration<PriceRule>
{
    public void Configure(EntityTypeBuilder<PriceRule> builder)
    {
        builder.ToTable("price_rules");

        builder.HasKey(pr => pr.PriceRuleId);

        builder.Property(pr => pr.PriceRuleId)
            .HasColumnName("price_rule_id")
            .HasColumnType("NUMBER(10)");

        builder.Property(pr => pr.TicketTypeId)
            .HasColumnName("ticket_type_id")
            .HasColumnType("NUMBER(10)")
            .IsRequired();

        builder.Property(pr => pr.RuleName)
            .HasColumnName("rule_name")
            .HasColumnType("VARCHAR2(100 CHAR)")
            .IsRequired();

        builder.Property(pr => pr.Priority)
            .HasColumnName("priority")
            .HasColumnType("NUMBER(5)")
            .IsRequired();

        builder.Property(pr => pr.EffectiveStartDate)
            .HasColumnName("effective_start_date")
            .HasColumnType("TIMESTAMP(0)")
            .IsRequired();

        builder.Property(pr => pr.EffectiveEndDate)
            .HasColumnName("effective_end_date")
            .HasColumnType("TIMESTAMP(0)")
            .IsRequired();

        builder.Property(pr => pr.MinQuantity)
            .HasColumnName("min_quantity")
            .HasColumnType("NUMBER(5)"); // 可空

        builder.Property(pr => pr.MaxQuantity)
            .HasColumnName("max_quantity")
            .HasColumnType("NUMBER(5)"); // 可空

        builder.Property(pr => pr.Price)
            .HasColumnName("price")
            .HasColumnType("NUMBER(10,2)")
            .IsRequired();

        // 添加 price >= 0 的 CHECK 约束
        builder.HasCheckConstraint("CK_price_rules_price", "price >= 0");

        builder.Property(pr => pr.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("TIMESTAMP(0)")
            .IsRequired()
            .HasDefaultValueSql("SYSTIMESTAMP");

        builder.Property(pr => pr.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("TIMESTAMP(0)")
            .IsRequired()
            .HasDefaultValueSql("SYSTIMESTAMP");

        builder.Property(pr => pr.CreatedById)
            .HasColumnName("created_by")
            .HasColumnType("NUMBER(10)"); // 可空

        // 配置索引
        builder.HasIndex(pr => pr.TicketTypeId);
        builder.HasIndex(pr => pr.Priority);
        builder.HasIndex(pr => pr.EffectiveStartDate);

        // 配置外键关系
        // 与 TicketType 的关系 (多对一)
        builder.HasOne(pr => pr.TicketType)
               .WithMany(tt => tt.PriceRules) // 关联到 TicketType 中的 PriceRules 集合
               .HasForeignKey(pr => pr.TicketTypeId)
               .OnDelete(DeleteBehavior.Cascade); // 如果票种被删除，相关的价格规则也应删除

        // 与 Employee 的关系 (多对一)
        builder.HasOne(pr => pr.CreatedBy)
               .WithMany() // 假设 Employee 实体没有 PriceRule 的集合
               .HasForeignKey(pr => pr.CreatedById)
               .OnDelete(DeleteBehavior.SetNull); // 如果员工被删除，将创建人ID设为null
    }
}


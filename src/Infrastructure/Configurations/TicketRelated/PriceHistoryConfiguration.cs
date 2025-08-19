using DbApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DbApp.Domain.Entities.TicketRelated;

namespace DbApp.Infrastructure.Configurations.TicketRelated
{
    public class PriceHistoryConfiguration : IEntityTypeConfiguration<PriceHistory>
    {
        public void Configure(EntityTypeBuilder<PriceHistory> builder)
        {
            builder.ToTable("price_histories");

            builder.HasKey(ph => ph.PriceHistoryId);

            builder.Property(ph => ph.PriceHistoryId)
                .HasColumnName("price_history_id")
                .HasColumnType("NUMBER(10)");

            builder.Property(ph => ph.TicketTypeId)
                .HasColumnName("ticket_type_id")
                .HasColumnType("NUMBER(10)")
                .IsRequired();

            builder.Property(ph => ph.PriceRuleId)
                .HasColumnName("price_rule_id")
                .HasColumnType("NUMBER(10)"); // 可空

            builder.Property(ph => ph.OldPrice)
                .HasColumnName("old_price")
                .HasColumnType("NUMBER(10,2)")
                .IsRequired();

            builder.Property(ph => ph.NewPrice)
                .HasColumnName("new_price")
                .HasColumnType("NUMBER(10,2)")
                .IsRequired();

            builder.Property(ph => ph.ChangeDatetime)
                .HasColumnName("change_datetime")
                .HasColumnType("TIMESTAMP(0)")
                .IsRequired()
                .HasDefaultValueSql("SYSTIMESTAMP");

            builder.Property(ph => ph.EmployeeId)
                .HasColumnName("employee_id")
                .HasColumnType("NUMBER(10)")
                .IsRequired();

            builder.Property(ph => ph.Reason)
                .HasColumnName("reason")
                .HasColumnType("VARCHAR2(500 CHAR)");

            builder.Property(ph => ph.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("TIMESTAMP(0)")
                .IsRequired()
                .HasDefaultValueSql("SYSTIMESTAMP");

            // 配置索引
            builder.HasIndex(ph => ph.TicketTypeId);
            builder.HasIndex(ph => ph.PriceRuleId);
            builder.HasIndex(ph => ph.ChangeDatetime);

            // 配置外键关系
            builder.HasOne(ph => ph.TicketType)
                   .WithMany() // 假设 TicketType 不需要 PriceHistory 集合的反向导航
                   .HasForeignKey(ph => ph.TicketTypeId)
                   .OnDelete(DeleteBehavior.Restrict); // 重要：历史记录不应随票种删除而删除，应禁止删除票种

            builder.HasOne(ph => ph.PriceRule)
                   .WithMany() // 假设 PriceRule 不需要 PriceHistory 集合
                   .HasForeignKey(ph => ph.PriceRuleId)
                   .OnDelete(DeleteBehavior.SetNull); // 如果价格规则被删除，历史记录中的关联ID设为NULL

            builder.HasOne(ph => ph.Employee)
                   .WithMany() // 假设 Employee 不需要 PriceHistory 集合
                   .HasForeignKey(ph => ph.EmployeeId)
                   .OnDelete(DeleteBehavior.Restrict); // 重要：不能删除执行过价格变更的员工
        }
    }
}
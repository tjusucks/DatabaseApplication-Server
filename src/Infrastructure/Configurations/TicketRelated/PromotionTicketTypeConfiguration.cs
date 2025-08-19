using DbApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DbApp.Domain.Entities.TicketRelated;

namespace DbApp.Infrastructure.Configurations.TicketRelated
{
    public class PromotionTicketTypeConfiguration : IEntityTypeConfiguration<PromotionTicketType>
    {
        public void Configure(EntityTypeBuilder<PromotionTicketType> builder)
        {
            builder.ToTable("promotion_ticket_types");

            // 1. 定义复合主键
            builder.HasKey(ptt => new { ptt.PromotionId, ptt.TicketTypeId });

            // 2. 配置各个列
            builder.Property(ptt => ptt.PromotionId)
                .HasColumnName("promotion_id")
                .HasColumnType("NUMBER(10)");

            builder.Property(ptt => ptt.TicketTypeId)
                .HasColumnName("ticket_type_id")
                .HasColumnType("NUMBER(10)");

            builder.Property(ptt => ptt.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("TIMESTAMP(0)")
                .IsRequired()
                .HasDefaultValueSql("SYSTIMESTAMP");

            // 3. 配置多对多关系
            // 配置与 Promotion 的关系
            builder.HasOne(ptt => ptt.Promotion)
                   .WithMany(p => p.ApplicableTicketTypes) // 对应 Promotion 实体中的集合
                   .HasForeignKey(ptt => ptt.PromotionId)
                   .OnDelete(DeleteBehavior.Cascade); // 如果活动被删除，其关联关系也应被删除

            // 配置与 TicketType 的关系
            builder.HasOne(ptt => ptt.TicketType)
                   .WithMany(tt => tt.ApplicablePromotions) // 对应 TicketType 实体中的集合
                   .HasForeignKey(ptt => ptt.TicketTypeId)
                   .OnDelete(DeleteBehavior.Cascade); // 如果票种被删除，其关联关系也应被删除
        }
    }
}
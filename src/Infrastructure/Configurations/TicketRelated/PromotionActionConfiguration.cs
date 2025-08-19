using DbApp.Domain.Entities.TicketRelated;
using DbApp.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DbApp.Infrastructure.Configurations.TicketRelated
{
public class PromotionActionConfiguration : IEntityTypeConfiguration<PromotionAction>
{
    public void Configure(EntityTypeBuilder<PromotionAction> builder)
    {
        builder.ToTable("promotion_actions");

        builder.HasKey(pa => pa.ActionId);
        
        builder.Property(pa => pa.ActionId).HasColumnName("action_id").HasColumnType("NUMBER(10)");
        builder.Property(pa => pa.PromotionId).HasColumnName("promotion_id").HasColumnType("NUMBER(10)").IsRequired();
        builder.Property(pa => pa.ActionName).HasColumnName("action_name").HasColumnType("VARCHAR2(100 CHAR)").IsRequired();

        builder.Property(pa => pa.ActionType)
            .HasColumnName("action_type")
            .HasColumnType("VARCHAR2(30 CHAR)")
            .IsRequired()
            .HasConversion(
                v => v.ToString().ToUpper(), // C# enum -> "PERCENT_OFF"
                v => (PromotionActionType)Enum.Parse(typeof(PromotionActionType), v.Replace("_", ""), true)
            );
        
        builder.HasCheckConstraint("CK_promotion_actions_action_type", 
            "action_type IN ('PERCENT_OFF', 'AMOUNT_OFF', 'FIXED_PRICE', 'FREE_TICKET', 'GIFT_POINTS')");
            
        builder.Property(pa => pa.TargetTicketTypeId).HasColumnName("target_ticket_type_id").HasColumnType("NUMBER(10)");

        // 配置带 CHECK 约束的字段
        builder.Property(pa => pa.DiscountPercentage).HasColumnName("discount_percentage").HasColumnType("NUMBER(5,2)");
        builder.HasCheckConstraint("CK_promotion_actions_discount_percentage", "discount_percentage BETWEEN 0 AND 100");

        builder.Property(pa => pa.DiscountAmount).HasColumnName("discount_amount").HasColumnType("NUMBER(10,2)");
        builder.HasCheckConstraint("CK_promotion_actions_discount_amount", "discount_amount >= 0");

        builder.Property(pa => pa.FixedPrice).HasColumnName("fixed_price").HasColumnType("NUMBER(10,2)");
        builder.HasCheckConstraint("CK_promotion_actions_fixed_price", "fixed_price >= 0");

        builder.Property(pa => pa.PointsAwarded).HasColumnName("points_awarded").HasColumnType("NUMBER(10)");
        builder.HasCheckConstraint("CK_promotion_actions_points_awarded", "points_awarded >= 0");

        builder.Property(pa => pa.FreeTicketTypeId).HasColumnName("free_ticket_type_id").HasColumnType("NUMBER(10)");
        
        builder.Property(pa => pa.FreeTicketQuantity).HasColumnName("free_ticket_quantity").HasColumnType("NUMBER(5)");
        builder.HasCheckConstraint("CK_promotion_actions_free_ticket_quantity", "free_ticket_quantity > 0");
        
        builder.Property(pa => pa.CreatedAt).HasColumnName("created_at").HasColumnType("TIMESTAMP(0)").IsRequired().HasDefaultValueSql("SYSTIMESTAMP");
        builder.Property(pa => pa.UpdatedAt).HasColumnName("updated_at").HasColumnType("TIMESTAMP(0)").IsRequired().HasDefaultValueSql("SYSTIMESTAMP");

        // 配置索引
        builder.HasIndex(pa => pa.PromotionId);
        builder.HasIndex(pa => pa.ActionType);
        builder.HasIndex(pa => pa.TargetTicketTypeId);

        // 配置外键
        builder.HasOne(pa => pa.Promotion)
               .WithMany(p => p.Actions)
               .HasForeignKey(pa => pa.PromotionId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pa => pa.TargetTicketType)
               .WithMany() // TicketType 不需要反向导航集合
               .HasForeignKey(pa => pa.TargetTicketTypeId)
               .OnDelete(DeleteBehavior.SetNull); // 目标票种被删除，将ID设为null

        builder.HasOne(pa => pa.FreeTicketType)
               .WithMany() // TicketType 不需要反向导航集合
               .HasForeignKey(pa => pa.FreeTicketTypeId)
               .OnDelete(DeleteBehavior.SetNull); // 赠送票种被删除，将ID设为null
    }
}
}
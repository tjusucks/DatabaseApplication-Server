using DbApp.Domain.Entities.TicketingSystem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace DbApp.Infrastructure.Configurations.TicketingSystem;

public class ReservationItemConfiguration : IEntityTypeConfiguration<ReservationItem>
{
    public void Configure(EntityTypeBuilder<ReservationItem> builder)
    {
        builder.ToTable("reservation_items");

        builder.HasKey(ri => ri.ItemId);

        builder.Property(ri => ri.ItemId).HasColumnName("item_id").HasColumnType("NUMBER(10)");
        builder.Property(ri => ri.ReservationId).HasColumnName("reservation_id").HasColumnType("NUMBER(10)").IsRequired();
        builder.Property(ri => ri.TicketTypeId).HasColumnName("ticket_type_id").HasColumnType("NUMBER(10)").IsRequired();

        builder.Property(ri => ri.Quantity).HasColumnName("quantity").HasColumnType("NUMBER(5)").IsRequired();
        builder.HasCheckConstraint("CK_reservation_items_quantity", "quantity > 0");

        builder.Property(ri => ri.UnitPrice).HasColumnName("unit_price").HasColumnType("NUMBER(10,2)").IsRequired();
        builder.HasCheckConstraint("CK_reservation_items_unit_price", "unit_price >= 0");

        builder.Property(ri => ri.AppliedPriceRuleId).HasColumnName("applied_price_rule_id").HasColumnType("NUMBER(10)");

        builder.Property(ri => ri.DiscountAmount).HasColumnName("discount_amount").HasColumnType("NUMBER(10,2)").IsRequired().HasDefaultValue(0);

        builder.Property(ri => ri.LineTotal).HasColumnName("line_total").HasColumnType("NUMBER(10,2)").IsRequired();
        builder.HasCheckConstraint("CK_reservation_items_line_total", "line_total >= 0");

        builder.Property(ri => ri.CreatedAt).HasColumnName("created_at").HasColumnType("TIMESTAMP(0)").IsRequired().HasDefaultValueSql("SYSTIMESTAMP");
        builder.Property(ri => ri.UpdatedAt).HasColumnName("updated_at").HasColumnType("TIMESTAMP(0)").IsRequired().HasDefaultValueSql("SYSTIMESTAMP");

        // 配置索引
        builder.HasIndex(ri => ri.ReservationId);
        builder.HasIndex(ri => ri.TicketTypeId);

        // 配置外键关系
        builder.HasOne(ri => ri.Reservation)
               .WithMany(r => r.ReservationItems) // 对应 Reservation 实体中的集合
               .HasForeignKey(ri => ri.ReservationId)
               .OnDelete(DeleteBehavior.Cascade); // 预订被删除，其下的项目也应一并删除

        builder.HasOne(ri => ri.TicketType)
               .WithMany() // TicketType 不需要反向导航
               .HasForeignKey(ri => ri.TicketTypeId)
               .OnDelete(DeleteBehavior.Restrict); // 禁止删除已被预订的票种

        builder.HasOne(ri => ri.AppliedPriceRule)
               .WithMany() // PriceRule 不需要反向导航
               .HasForeignKey(ri => ri.AppliedPriceRuleId)
               .OnDelete(DeleteBehavior.SetNull); // 价格规则被删除，将ID设为null

        // 配置与 Ticket 的一对多关系
        builder.HasMany(ri => ri.Tickets)
               .WithOne(t => t.ReservationItem)
               .HasForeignKey(t => t.ReservationItemId);
    }
}

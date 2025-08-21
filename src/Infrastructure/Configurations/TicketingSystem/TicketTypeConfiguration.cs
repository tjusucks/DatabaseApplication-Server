using DbApp.Domain.Entities.TicketingSystem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DbApp.Infrastructure.Configurations.TicketingSystem;

public class TicketTypeConfiguration : IEntityTypeConfiguration<TicketType>
{
    public void Configure(EntityTypeBuilder<TicketType> builder)
    {
        builder.ToTable("ticket_types");

        builder.HasKey(tt => tt.TicketTypeId);

        builder.Property(tt => tt.TicketTypeId)
            .HasColumnName("ticket_type_id")
            .HasColumnType("NUMBER(10)");

        builder.Property(tt => tt.TypeName)
            .HasColumnName("type_name")
            .HasColumnType("VARCHAR2(100 CHAR)")
            .IsRequired();

        // 为 type_name 创建唯一索引
        builder.HasIndex(tt => tt.TypeName)
            .IsUnique();

        builder.Property(tt => tt.Description)
            .HasColumnName("description")
            .HasColumnType("VARCHAR2(4000 CHAR)"); // 可空

        builder.Property(tt => tt.BasePrice)
            .HasColumnName("base_price")
            .HasColumnType("NUMBER(10,2)")
            .IsRequired();

        builder.Property(tt => tt.ApplicableCrowd)
            .HasColumnName("applicable_crowd")
            .IsRequired();

        builder.Property(tt => tt.RulesText)
            .HasColumnName("rules_text")
            .HasColumnType("VARCHAR2(4000 CHAR)"); // 可空

        builder.Property(tt => tt.MaxSaleLimit)
            .HasColumnName("max_sale_limit")
            .HasColumnType("NUMBER(10)"); // 可空

        builder.Property(tt => tt.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("TIMESTAMP(0)")
            .IsRequired()
            .HasDefaultValueSql("SYSTIMESTAMP");

        builder.Property(tt => tt.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("TIMESTAMP(0)")
            .IsRequired()
            .HasDefaultValueSql("SYSTIMESTAMP");

        // 配置一对多关系
        // TicketType (一) -> Ticket (多)
        builder.HasMany(tt => tt.Tickets)
               .WithOne(t => t.TicketType) // 在 Ticket 实体中，与 TicketType 导航属性关联
               .HasForeignKey(t => t.TicketTypeId); // Ticket 表中的外键
    }
}

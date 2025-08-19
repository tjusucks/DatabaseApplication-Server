// 位置: Infrastructure/Configurations/TicketTypeConfiguration.cs

using DbApp.Domain.Entities;
using DbApp.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DbApp.Domain.Entities.TicketRelated;

namespace DbApp.Infrastructure.Configurations.TicketRelated{
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

            // 添加 base_price >= 0 的 CHECK 约束
            builder.HasCheckConstraint("CK_ticket_types_base_price", "base_price >= 0");

            builder.Property(tt => tt.ApplicableCrowd)
                .HasColumnName("applicable_crowd")
                .HasColumnType("VARCHAR2(30 CHAR)")
                .IsRequired()
                .HasConversion(
                    // 将 C# enum 转换为数据库中的大写字符串，以匹配 CHECK 约束
                    v => v.ToString().ToUpper(),
                    // 将数据库中的字符串转换回 C# enum (忽略大小写)
                    v => (ApplicableCrowd)Enum.Parse(typeof(ApplicableCrowd), v, true)
                );

            // 添加 applicable_crowd 的 CHECK 约束
            builder.HasCheckConstraint("CK_ticket_types_applicable_crowd", "applicable_crowd IN ('ADULT', 'CHILD', 'SENIOR', 'FAMILY', 'ANY')");

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
}
using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Enums.UserSystem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DbApp.Infrastructure.Configurations.UserSystem;

/// <summary>
/// Entity Framework configuration for the Visitor entity.
/// Configures the visitors table structure and relationships.
/// </summary>
public class VisitorConfiguration : IEntityTypeConfiguration<Visitor>
{
    public void Configure(EntityTypeBuilder<Visitor> builder)
    {
        // Table name.
        builder.ToTable("visitors");

        // Primary key (also foreign key to users table).
        builder.HasKey(v => v.VisitorId);

        builder.Property(v => v.VisitorId)
            .HasColumnName("visitor_id")
            .HasColumnType("NUMBER(10)");

        // Visitor type with default value and check constraint.
        builder.Property(v => v.VisitorType)
            .HasColumnName("visitor_type")
            .IsRequired()
            .HasDefaultValue(VisitorType.Regular);

        // Membership points.
        builder.Property(v => v.Points)
            .HasColumnName("points")
            .HasColumnType("NUMBER(10)")
            .HasDefaultValue(0);

        // Member level - optional.
        builder.Property(v => v.MemberLevel)
            .HasColumnName("member_level")
            .HasColumnType("VARCHAR2(30 CHAR)");

        // Member since date.
        builder.Property(v => v.MemberSince)
            .HasColumnName("member_since")
            .HasColumnType("TIMESTAMP(0)");

        // Blacklist flag.
        builder.Property(v => v.IsBlacklisted)
            .HasColumnName("is_blacklisted")
            .HasColumnType("NUMBER(1)")
            .IsRequired()
            .HasDefaultValue(false);

        // Height for ride restrictions.
        builder.Property(v => v.Height)
            .HasColumnName("height")
            .HasColumnType("NUMBER(10)")
            .IsRequired();

        // Audit fields.
        builder.Property(v => v.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("TIMESTAMP(0)")
            .IsRequired()
            .HasDefaultValueSql("SYSTIMESTAMP");

        builder.Property(v => v.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("TIMESTAMP(0)")
            .HasDefaultValueSql("SYSTIMESTAMP");

        // Foreign key relationship to users table.
        builder.HasOne<User>()
            .WithOne()
            .HasForeignKey<Visitor>(v => v.VisitorId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes.
        builder.HasIndex(v => v.VisitorType);

        builder.HasIndex(v => v.IsBlacklisted);

        builder.HasIndex(v => v.Height);
    }
}

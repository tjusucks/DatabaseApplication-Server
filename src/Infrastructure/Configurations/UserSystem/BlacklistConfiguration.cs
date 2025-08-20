using DbApp.Domain.Entities.UserSystem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DbApp.Infrastructure.Configurations.UserSystem;

/// <summary>
/// Entity Framework configuration for the Blacklist entity.
/// Configures the blacklist table structure and relationships.
/// </summary>
public class BlacklistConfiguration : IEntityTypeConfiguration<Blacklist>
{
    public void Configure(EntityTypeBuilder<Blacklist> builder)
    {
        // Table name.
        builder.ToTable("blacklist");

        // Primary key (also foreign key to visitors table).
        builder.HasKey(b => b.VisitorId);

        builder.Property(b => b.VisitorId)
            .HasColumnName("visitor_id")
            .HasColumnType("NUMBER(10)");

        // Blacklist reason.
        builder.Property(b => b.BlacklistReason)
            .HasColumnName("blacklist_reason")
            .HasColumnType("VARCHAR2(500 CHAR)");

        // Audit fields.
        builder.Property(b => b.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("TIMESTAMP(0)")
            .IsRequired()
            .HasDefaultValueSql("SYSTIMESTAMP");

        builder.Property(b => b.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("TIMESTAMP(0)")
            .HasDefaultValueSql("SYSTIMESTAMP");

        // Foreign key relationship to visitors table.
        builder.HasOne(b => b.Visitor)
            .WithOne()
            .HasForeignKey<Blacklist>(b => b.VisitorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

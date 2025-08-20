using DbApp.Domain.Entities.UserSystem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DbApp.Infrastructure.Configurations.UserSystem;

/// <summary>
/// Entity Framework configuration for the Role entity.
/// Configures the roles table structure.
/// </summary>
public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        // Table name.
        builder.ToTable("roles");

        // Primary key.
        builder.HasKey(r => r.RoleId);

        // Primary key configuration.
        builder.Property(r => r.RoleId)
            .HasColumnName("role_id")
            .HasColumnType("NUMBER(10)")
            .ValueGeneratedOnAdd();

        // Role name - unique and required.
        builder.Property(r => r.RoleName)
            .HasColumnName("role_name")
            .HasColumnType("VARCHAR2(50 CHAR)")
            .IsRequired();

        // Role description - optional.
        builder.Property(r => r.RoleDescription)
            .HasColumnName("role_description")
            .HasColumnType("VARCHAR2(500 CHAR)");

        // System role flag.
        builder.Property(r => r.IsSystemRole)
            .HasColumnName("is_system_role")
            .HasColumnType("NUMBER(1)")
            .IsRequired()
            .HasDefaultValue(false);

        // Audit fields.
        builder.Property(r => r.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("TIMESTAMP(0)")
            .IsRequired()
            .HasDefaultValueSql("SYSTIMESTAMP");

        builder.Property(r => r.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("TIMESTAMP(0)")
            .HasDefaultValueSql("SYSTIMESTAMP");

        // Navigation property for foreign key relationship.
        builder.HasMany(r => r.Users)
            .WithOne(u => u.Role)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index.
        builder.HasIndex(r => r.RoleName)
            .IsUnique();
    }
}

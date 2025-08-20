using DbApp.Domain.Entities.UserSystem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DbApp.Infrastructure.Configurations.UserSystem;

/// <summary>
/// Entity Framework configuration for the User entity.
/// Configures the users table structure and relationships.
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Table name.
        builder.ToTable("users");

        // Primary key.
        builder.HasKey(u => u.UserId);

        builder.Property(u => u.UserId)
            .HasColumnName("user_id")
            .HasColumnType("NUMBER(10)")
            .ValueGeneratedOnAdd();

        // Username - unique and required.
        builder.Property(u => u.Username)
            .HasColumnName("username")
            .HasColumnType("VARCHAR2(50 CHAR)")
            .IsRequired();

        // Password hash - required.
        builder.Property(u => u.PasswordHash)
            .HasColumnName("password_hash")
            .HasColumnType("VARCHAR2(256 CHAR)")
            .IsRequired();

        // Email - unique and required.
        builder.Property(u => u.Email)
            .HasColumnName("email")
            .HasColumnType("VARCHAR2(100 CHAR)")
            .IsRequired();

        // Display name - required.
        builder.Property(u => u.DisplayName)
            .HasColumnName("display_name")
            .HasColumnType("VARCHAR2(50 CHAR)")
            .IsRequired();

        // Phone number - optional.
        builder.Property(u => u.PhoneNumber)
            .HasColumnName("phone_number")
            .HasColumnType("VARCHAR2(20 CHAR)");

        // Birth date - optional.
        builder.Property(u => u.BirthDate)
            .HasColumnName("birth_date")
            .HasColumnType("TIMESTAMP(0)");

        // Gender - optional with check constraint.
        builder.Property(u => u.Gender)
            .HasColumnName("gender");

        // Register time.
        builder.Property(u => u.RegisterTime)
            .HasColumnName("register_time")
            .HasColumnType("TIMESTAMP(0)")
            .IsRequired()
            .HasDefaultValueSql("SYSTIMESTAMP");

        // Permission level.
        // Use NUMBER(2) because NUMBER(1) will be converted to bool by EF Core.
        builder.Property(u => u.PermissionLevel)
            .HasColumnName("permission_level")
            .HasColumnType("NUMBER(2)")
            .IsRequired()
            .HasDefaultValue(0);

        // Role ID - foreign key.
        builder.Property(u => u.RoleId)
            .HasColumnName("role_id")
            .HasColumnType("NUMBER(10)")
            .IsRequired();

        // Audit fields.
        builder.Property(u => u.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("TIMESTAMP(0)")
            .IsRequired()
            .HasDefaultValueSql("SYSTIMESTAMP");

        builder.Property(u => u.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("TIMESTAMP(0)")
            .HasDefaultValueSql("SYSTIMESTAMP");

        // Navigation property for foreign key relationship to roles table.
        builder.HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes.
        builder.HasIndex(u => u.Username)
            .IsUnique();

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.HasIndex(u => u.CreatedAt);

        builder.HasIndex(u => u.RoleId);
    }
}

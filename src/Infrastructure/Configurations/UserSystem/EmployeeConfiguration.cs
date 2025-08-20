using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Enums.UserSystem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DbApp.Infrastructure.Configurations.UserSystem;

/// <summary>
/// Entity Framework configuration for the Employee entity.
/// Configures the employees table structure and relationships.
/// </summary>
public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        // Table name.
        builder.ToTable("employees");

        // Primary key (also foreign key to users table).
        builder.HasKey(e => e.EmployeeId);

        builder.Property(e => e.EmployeeId)
            .HasColumnName("employee_id")
            .HasColumnType("NUMBER(10)");

        // Staff number - unique identifier.
        builder.Property(e => e.StaffNumber)
            .HasColumnName("staff_number")
            .HasColumnType("VARCHAR2(20 CHAR)")
            .IsRequired();

        // Position.
        builder.Property(e => e.Position)
            .HasColumnName("position")
            .HasColumnType("VARCHAR2(50 CHAR)")
            .IsRequired();

        // Department name.
        builder.Property(e => e.DepartmentName)
            .HasColumnName("department_name")
            .HasColumnType("VARCHAR2(50 CHAR)");

        // Staff type with check constraint.
        builder.Property(e => e.StaffType)
            .HasColumnName("staff_type");

        // Team ID - foreign key to staff_teams.
        builder.Property(e => e.TeamId)
            .HasColumnName("team_id")
            .HasColumnType("NUMBER(10)");

        // Hire date.
        builder.Property(e => e.HireDate)
            .HasColumnName("hire_date")
            .HasColumnType("TIMESTAMP(0)")
            .IsRequired()
            .HasDefaultValueSql("SYSTIMESTAMP");

        // Employment status.
        builder.Property(e => e.EmploymentStatus)
            .HasColumnName("employment_status")
            .IsRequired()
            .HasDefaultValue(EmploymentStatus.Active);

        // Manager ID - self-referencing foreign key.
        builder.Property(e => e.ManagerId)
            .HasColumnName("manager_id")
            .HasColumnType("NUMBER(10)");

        // Certification information.
        builder.Property(e => e.Certification)
            .HasColumnName("certification")
            .HasColumnType("VARCHAR2(500 CHAR)");

        // Responsibility area.
        builder.Property(e => e.ResponsibilityArea)
            .HasColumnName("responsibility_area")
            .HasColumnType("VARCHAR2(100 CHAR)");

        // Audit fields.
        builder.Property(e => e.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("TIMESTAMP(0)")
            .IsRequired()
            .HasDefaultValueSql("SYSTIMESTAMP");

        builder.Property(e => e.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("TIMESTAMP(0)")
            .HasDefaultValueSql("SYSTIMESTAMP");

        // Foreign key relationship to users table.
        builder.HasOne<User>()
            .WithOne()
            .HasForeignKey<Employee>(e => e.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Self-referencing foreign key for manager.
        builder.HasOne<Employee>()
            .WithMany()
            .HasForeignKey(e => e.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Foreign key relationship to staff_teams table.
        builder.HasOne<StaffTeam>()
            .WithMany()
            .HasForeignKey(e => e.TeamId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes.
        builder.HasIndex(e => e.StaffNumber)
            .IsUnique();

        builder.HasIndex(e => e.DepartmentName);
    }
}

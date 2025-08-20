using DbApp.Domain.Entities.UserSystem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DbApp.Infrastructure.Configurations.UserSystem;

/// <summary>
/// Entity Framework configuration for the StaffTeam entity.
/// Configures the staff_teams table structure and relationships.
/// </summary>
public class StaffTeamConfiguration : IEntityTypeConfiguration<StaffTeam>
{
    public void Configure(EntityTypeBuilder<StaffTeam> builder)
    {
        // Table name.
        builder.ToTable("staff_teams");

        // Primary key.
        builder.HasKey(st => st.TeamId);

        builder.Property(st => st.TeamId)
            .HasColumnName("team_id")
            .HasColumnType("NUMBER(10)")
            .ValueGeneratedOnAdd();

        // Team name.
        builder.Property(st => st.TeamName)
            .HasColumnName("team_name")
            .HasColumnType("VARCHAR2(50 CHAR)")
            .IsRequired();

        // Team type with check constraint.
        builder.Property(st => st.TeamType)
            .HasColumnName("team_type")
            .IsRequired();

        // Leader ID - foreign key to employees.
        builder.Property(st => st.LeaderId)
            .HasColumnName("leader_id")
            .HasColumnType("NUMBER(10)")
            .IsRequired();

        // Audit fields.
        builder.Property(st => st.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("TIMESTAMP(0)")
            .IsRequired()
            .HasDefaultValueSql("SYSTIMESTAMP");

        builder.Property(st => st.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("TIMESTAMP(0)")
            .HasDefaultValueSql("SYSTIMESTAMP");

        // Foreign key relationship to employees table for leader.
        builder.HasOne<Employee>()
            .WithMany()
            .HasForeignKey(st => st.LeaderId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes.
        builder.HasIndex(st => st.TeamType);
    }
}

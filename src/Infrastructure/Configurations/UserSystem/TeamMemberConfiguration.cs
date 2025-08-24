using DbApp.Domain.Entities.UserSystem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DbApp.Infrastructure.Configurations.UserSystem;

/// <summary>
/// Entity Framework configuration for the TeamMember entity.
/// Configures the team_members table structure and many-to-many relationships.
/// </summary>
public class TeamMemberConfiguration : IEntityTypeConfiguration<TeamMember>
{
    public void Configure(EntityTypeBuilder<TeamMember> builder)
    {
        // Table name.
        builder.ToTable("team_members");

        // Composite primary key.
        builder.HasKey(tm => new { tm.TeamId, tm.EmployeeId });

        // Team ID.
        builder.Property(tm => tm.TeamId)
            .HasColumnName("team_id")
            .HasColumnType("NUMBER(10)")
            .IsRequired();

        // Employee ID.
        builder.Property(tm => tm.EmployeeId)
            .HasColumnName("employee_id")
            .HasColumnType("NUMBER(10)")
            .IsRequired();

        // Join date.
        builder.Property(tm => tm.JoinDate)
            .HasColumnName("join_date")
            .HasColumnType("TIMESTAMP")
            .IsRequired()
            .HasDefaultValueSql("SYSTIMESTAMP");

        // Navigation properties for foreign key relationships.
        builder.HasOne(tm => tm.Team)
            .WithMany(st => st.TeamMembers)
            .HasForeignKey(tm => tm.TeamId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(tm => tm.Employee)
            .WithMany()
            .HasForeignKey(tm => tm.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Additional indexes for performance.
        builder.HasIndex(tm => tm.TeamId);

        builder.HasIndex(tm => tm.EmployeeId);
    }
}

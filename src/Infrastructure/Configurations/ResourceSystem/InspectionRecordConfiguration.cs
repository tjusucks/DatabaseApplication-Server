using DbApp.Domain.Entities.ResourceSystem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DbApp.Infrastructure.Configurations.ResourceSystem;

public class InspectionRecordConfiguration : IEntityTypeConfiguration<InspectionRecord>
{
    public void Configure(EntityTypeBuilder<InspectionRecord> builder)
    {
        // 表名和基础配置
        builder.ToTable("inspection_records");
        builder.HasKey(r => r.InspectionId);

        // 属性映射
        builder.Property(r => r.InspectionId)
            .HasColumnName("inspection_id")
            .HasColumnType("NUMBER(10)");

        builder.Property(r => r.RideId)
            .HasColumnName("ride_id")
            .HasColumnType("NUMBER(10)");

        builder.Property(r => r.TeamId)
            .HasColumnName("team_id")
            .HasColumnType("NUMBER(10)");

        builder.Property(r => r.CheckDate)
            .HasColumnName("check_date")
            .HasColumnType("TIMESTAMP(0)");

        builder.Property(r => r.CheckType)
            .IsRequired()
            .HasColumnName("check_type");

        builder.Property(r => r.IsPassed)
            .HasColumnName("is_passed")
            .HasColumnType("NUMBER(1)");

        builder.Property(r => r.IssuesFound)
            .HasColumnName("issues_found")
            .HasColumnType("VARCHAR2(4000 CHAR)");

        builder.Property(r => r.Recommendations)
            .HasColumnName("recommendations")
            .HasColumnType("VARCHAR2(4000 CHAR)");

        builder.Property(r => r.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("TIMESTAMP(0)")
            .HasDefaultValueSql("SYSTIMESTAMP");

        builder.Property(r => r.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("TIMESTAMP(0)")
            .HasDefaultValueSql("SYSTIMESTAMP");

        // 索引配置
        builder.HasIndex(r => r.CheckDate);
        builder.HasIndex(r => r.IsPassed);
        builder.HasIndex(r => r.RideId);
        builder.HasIndex(r => r.TeamId);

        // 关系配置
        // 与游乐设施的关系
        builder.HasOne(r => r.Ride)
            .WithMany(a => a.InspectionRecords)
            .HasForeignKey(r => r.RideId)
            .IsRequired();

        // 与检查组的关系
        builder.HasOne(r => r.Team)
            .WithMany(t => t.InspectionRecords)
            .HasForeignKey(r => r.TeamId)
            .IsRequired();
    }
}

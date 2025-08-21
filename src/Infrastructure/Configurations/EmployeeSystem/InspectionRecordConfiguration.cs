using DbApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DbApp.Infrastructure.Configurations;

public class InspectionRecordConfiguration : IEntityTypeConfiguration<InspectionRecord>
{
    public void Configure(EntityTypeBuilder<InspectionRecord> builder)
    {
        // 表名和基础配置
        builder.ToTable("INSPECTION_RECORDS");
        builder.HasKey(r => r.InspectionId);

        // 属性映射
        builder.Property(r => r.InspectionId)
            .HasColumnName("INSPECTION_ID")
            .HasPrecision(10);

        builder.Property(r => r.RideId)
            .HasColumnName("RIDE_ID")
            .HasPrecision(10);

        builder.Property(r => r.TeamId)
            .HasColumnName("TEAM_ID")
            .HasPrecision(10);

        builder.Property(r => r.CheckDate)
            .HasColumnName("CHECK_DATE");

        builder.Property(r => r.CheckType)
            .IsRequired()
            .HasColumnName("CHECK_TYPE")
            .HasMaxLength(30)
            .IsUnicode(false)
            .HasConversion<string>();

        builder.Property(r => r.IsPassed)
            .HasColumnName("IS_PASSED")
            .HasColumnType("NUMBER(1)");

        builder.Property(r => r.IssuesFound)
            .HasColumnName("ISSUES_FOUND")
            .IsUnicode(false);

        builder.Property(r => r.Recommendations)
            .HasColumnName("RECOMMENDATIONS")
            .IsUnicode(false);

        builder.Property(r => r.CreatedAt)
            .HasColumnName("CREATED_AT");

        builder.Property(r => r.UpdatedAt)
            .HasColumnName("UPDATED_AT");

        // 索引配置
        builder.HasIndex(r => r.CheckDate, "INSPECTION_RECORDS_CHECK_DATE_IDX");
        builder.HasIndex(r => r.IsPassed, "INSPECTION_RECORDS_IS_PASSED_IDX");
        builder.HasIndex(r => r.RideId, "INSPECTION_RECORDS_RIDE_ID_IDX");
        builder.HasIndex(r => r.TeamId, "INSPECTION_RECORDS_TEAM_ID_IDX");

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

using DbApp.Domain.Entities.ResourceSystem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DbApp.Infrastructure.Configurations.ResourceSystem;

public class EmployeeReviewConfiguration : IEntityTypeConfiguration<EmployeeReview>
{
    public void Configure(EntityTypeBuilder<EmployeeReview> builder)
    {
        // 表名和基础配置
        builder.ToTable("employee_reviews", r =>
        {
            r.HasCheckConstraint("CK_employee_reviews_score_Range", "\"score\" BETWEEN 0.0 AND 100.0");
        });

        builder.HasKey(r => r.ReviewId);

        // 属性映射
        builder.Property(r => r.ReviewId)
            .HasColumnName("review_id")
            .HasColumnType("NUMBER(10)");

        builder.Property(r => r.EmployeeId)
            .HasColumnName("employee_id")
            .HasColumnType("NUMBER(10)");

        builder.Property(r => r.Period)
            .IsRequired()
            .HasColumnName("period")
            .HasColumnType("VARCHAR2(7)");

        builder.Property(r => r.Score)
            .HasColumnName("score")
            .HasColumnType("NUMBER(5,2)");

        builder.Property(r => r.EvaluationLevel)
            .HasColumnName("evaluation_level");

        builder.Property(r => r.EvaluatorId)
            .HasColumnName("evaluator_id")
            .HasColumnType("NUMBER(10)");

        builder.Property(r => r.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("TIMESTAMP(0)")
            .HasDefaultValueSql("SYSTIMESTAMP");

        builder.Property(r => r.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("TIMESTAMP(0)")
            .HasDefaultValueSql("SYSTIMESTAMP");

        // 索引配置
        builder.HasIndex(r => r.EmployeeId);
        builder.HasIndex(r => r.Period);

        // 关系配置
        builder.HasOne(r => r.Employee)
            .WithMany(e => e.Reviews)
            .HasForeignKey(r => r.EmployeeId);

        builder.HasOne(r => r.Evaluator)
            .WithMany(e => e.EvaluatedReviews)
            .HasForeignKey(r => r.EvaluatorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

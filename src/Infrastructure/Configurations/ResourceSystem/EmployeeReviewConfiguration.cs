using DbApp.Domain.Entities.ResourceSystem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DbApp.Infrastructure.Configurations.ResourceSystem;

public class EmployeeReviewConfiguration : IEntityTypeConfiguration<EmployeeReview>
{
    public void Configure(EntityTypeBuilder<EmployeeReview> builder)
    {
        // 表名和基础配置
        builder.ToTable("EMPLOYEE_REVIEWS");
        builder.HasKey(r => r.ReviewId);

        // 属性映射
        builder.Property(r => r.ReviewId)
            .HasColumnName("REVIEW_ID")
            .HasPrecision(10);

        builder.Property(r => r.EmployeeId)
            .HasColumnName("EMPLOYEE_ID")
            .HasPrecision(10);

        builder.Property(r => r.Period)
            .IsRequired()
            .HasColumnName("PERIOD")
            .HasMaxLength(7)
            .IsUnicode(false);

        builder.Property(r => r.Score)
            .HasColumnName("SCORE")
            .HasColumnType("decimal(5,2)");

        builder.Property(r => r.EvaluationLevel)
            .HasColumnName("EVALUATION_LEVEL")
            .HasMaxLength(30)
            .IsUnicode(false)
            .HasConversion<string>();

        builder.Property(r => r.EvaluatorId)
            .HasColumnName("EVALUATOR_ID")
            .HasPrecision(10);

        builder.Property(r => r.CreatedAt)
            .HasColumnName("CREATED_AT");

        builder.Property(r => r.UpdatedAt)
            .HasColumnName("UPDATED_AT");

        // 索引配置
        builder.HasIndex(r => r.EmployeeId, "EMPLOYEE_REVIEWS_EMPLOYEE_ID_IDX");
        builder.HasIndex(r => r.Period, "EMPLOYEE_REVIEWS_PERIOD_IDX");

        // 关系配置
        // 与被评价员工的关系
        //builder.HasOne(r => r.Employee)
        //.WithMany(e => e.EmployeeReviewEmployees)
        //.HasForeignKey(r => r.EmployeeId)
        //.OnDelete(DeleteBehavior.Restrict); // 避免级联删除冲突

        // 与评价人员工的关系
        //builder.HasOne(r => r.Evaluator)
        //.WithMany(e => e.EmployeeReviewEvaluators)
        //.HasForeignKey(r => r.EvaluatorId)
        //.OnDelete(DeleteBehavior.Restrict); // 避免级联删除冲突
    }
}

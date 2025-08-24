using DbApp.Domain.Entities.ResourceSystem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DbApp.Infrastructure.Configurations.ResourceSystem;

public class SalaryRecordConfiguration : IEntityTypeConfiguration<SalaryRecord>
{
    public void Configure(EntityTypeBuilder<SalaryRecord> builder)
    {
        // 表名和基础配置
        builder.ToTable("salary_records", r =>
        {
            r.HasCheckConstraint("CK_salary_records_salary_Range", "\"salary\" >= 0");
        });

        builder.HasKey(r => r.SalaryRecordId);

        // 属性映射
        builder.Property(r => r.SalaryRecordId)
            .HasColumnName("salary_record_id")
            .HasColumnType("NUMBER(10)");

        builder.Property(r => r.EmployeeId)
            .HasColumnName("employee_id")
            .HasColumnType("NUMBER(10)");

        builder.Property(r => r.PayDate)
            .HasColumnName("pay_date")
            .HasColumnType("TIMESTAMP(0)");

        builder.Property(r => r.Salary)
            .HasColumnName("salary")
            .HasColumnType("NUMBER(10,2)");

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
        builder.HasIndex(r => r.PayDate);

        // 关系配置
        builder.HasOne(r => r.Employee)
            .WithMany(e => e.SalaryRecords)
            .HasForeignKey(r => r.EmployeeId)
            .IsRequired();
    }
}

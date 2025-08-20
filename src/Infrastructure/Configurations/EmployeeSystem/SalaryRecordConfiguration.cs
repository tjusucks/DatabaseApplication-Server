using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DbApp.Domain.Entities;

namespace DbApp.Infrastructure.Configurations;

public class SalaryRecordConfiguration : IEntityTypeConfiguration<SalaryRecord>
{
    public void Configure(EntityTypeBuilder<SalaryRecord> builder)
    {
        // 表名和基础配置
        builder.ToTable("SALARY_RECORDS");
        builder.HasKey(r => r.SalaryRecordId);

        // 属性映射
        builder.Property(r => r.SalaryRecordId)
            .HasColumnName("SALARY_RECORD_ID")
            .HasPrecision(10);

        builder.Property(r => r.EmployeeId)
            .HasColumnName("EMPLOYEE_ID")
            .HasPrecision(10);

        builder.Property(r => r.PayDate)
            .HasColumnName("PAY_DATE");

        builder.Property(r => r.Salary)
            .HasColumnName("SALARY")
            .HasColumnType("decimal(10,2)"); // 精确到小数点后两位

        builder.Property(r => r.CreatedAt)
            .HasColumnName("CREATED_AT");

        builder.Property(r => r.UpdatedAt)
            .HasColumnName("UPDATED_AT");

        // 索引配置
        builder.HasIndex(r => r.EmployeeId, "SALARY_RECORDS_EMPLOYEE_ID_IDX");
        builder.HasIndex(r => r.PayDate, "SALARY_RECORDS_PAY_DATE_IDX");

        // 关系配置
        builder.HasOne(r => r.Employee)
            .WithMany(e => e.SalaryRecords)
            .HasForeignKey(r => r.EmployeeId)
            .IsRequired();
    }
}
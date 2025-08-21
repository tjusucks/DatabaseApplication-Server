using DbApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DbApp.Infrastructure.Configurations;

public class FinancialRecordConfiguration : IEntityTypeConfiguration<FinancialRecord>
{
    public void Configure(EntityTypeBuilder<FinancialRecord> builder)
    {
        // 表名和基础配置
        builder.ToTable("FINANCIAL_RECORDS");
        builder.HasKey(r => r.RecordId);

        // 属性映射
        builder.Property(r => r.RecordId)
            .HasColumnName("RECORD_ID")
            .HasPrecision(10);

        builder.Property(r => r.TransactionDate)
            .HasColumnName("TRANSACTION_DATE");

        builder.Property(r => r.Amount)
            .HasColumnName("AMOUNT")
            .HasColumnType("decimal(12,2)"); // 12位精度，2位小数

        builder.Property(r => r.TransactionType)
            .IsRequired()
            .HasColumnName("TRANSACTION_TYPE")
            .HasMaxLength(30)
            .IsUnicode(false)
            .HasConversion<string>();

        builder.Property(r => r.PaymentMethod)
            .HasColumnName("PAYMENT_METHOD")
            .HasMaxLength(30)
            .IsUnicode(false)
            .HasConversion<string>();

        builder.Property(r => r.ResponsibleEmployeeId)
            .HasColumnName("RESPONSIBLE_EMPLOYEE_ID")
            .HasPrecision(10);

        builder.Property(r => r.ApprovedBy)
            .HasColumnName("APPROVED_BY")
            .HasPrecision(10);

        builder.Property(r => r.CreatedAt)
            .HasColumnName("CREATED_AT");

        builder.Property(r => r.UpdatedAt)
            .HasColumnName("UPDATED_AT");

        // 索引配置（4个索引）
        builder.HasIndex(r => r.ApprovedBy, "FINANCIAL_RECORDS_APPROVED_BY_IDX");
        builder.HasIndex(r => r.ResponsibleEmployeeId, "FINANCIAL_RECORDS_RESPONSIBLE_EMPLOYEE_ID_IDX");
        builder.HasIndex(r => r.TransactionDate, "FINANCIAL_RECORDS_TRANSACTION_DATE_IDX");
        builder.HasIndex(r => r.TransactionType, "FINANCIAL_RECORDS_TRANSACTION_TYPE_IDX");

        // 关系配置
        // 与审批人员工的关系
        //builder.HasOne(r => r.ApprovedByNavigation)
        //.WithMany(e => e.FinancialRecordApprovedByNavigations)
        //.HasForeignKey(r => r.ApprovedBy);

        // 与负责人员工的关系
        //builder.HasOne(r => r.ResponsibleEmployee)
        //.WithMany(e => e.FinancialRecordResponsibleEmployees)
        //.HasForeignKey(r => r.ResponsibleEmployeeId);
    }
}

using DbApp.Domain.Entities.ResourceSystem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DbApp.Infrastructure.Configurations.ResourceSystem;

public class FinancialRecordConfiguration : IEntityTypeConfiguration<FinancialRecord>
{
    public void Configure(EntityTypeBuilder<FinancialRecord> builder)
    {
        // 表名和基础配置
        builder.ToTable("financial_records");
        builder.HasKey(r => r.RecordId);

        // 属性映射
        builder.Property(r => r.RecordId)
            .HasColumnName("record_id")
            .HasColumnType("NUMBER(10)");

        builder.Property(r => r.TransactionDate)
            .HasColumnName("transaction_date")
            .HasColumnType("TIMESTAMP(0)");

        builder.Property(r => r.Amount)
            .HasColumnName("amount")
            .HasColumnType("NUMBER(12,2)");

        builder.Property(r => r.TransactionType)
            .IsRequired()
            .HasColumnName("transaction_type");

        builder.Property(r => r.PaymentMethod)
            .HasColumnName("payment_method");

        builder.Property(r => r.ResponsibleEmployeeId)
            .HasColumnName("responsible_employee_id")
            .HasColumnType("NUMBER(10)");

        builder.Property(r => r.ApprovedById)
            .HasColumnName("approved_by_id")
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
        builder.HasIndex(r => r.ApprovedById);
        builder.HasIndex(r => r.ResponsibleEmployeeId);
        builder.HasIndex(r => r.TransactionDate);
        builder.HasIndex(r => r.TransactionType);

        // 关系配置
        // 与审批人员工的关系
        builder.HasOne(r => r.ApprovedBy)
            .WithMany()
            .HasForeignKey(r => r.ApprovedById)
            .OnDelete(DeleteBehavior.Restrict);

        // 与负责人员工的关系
        builder.HasOne(r => r.ResponsibleEmployee)
            .WithMany()
            .HasForeignKey(r => r.ResponsibleEmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

using DbApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DbApp.Infrastructure.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        // 表名和基础配置
        builder.ToTable("EMPLOYEES");
        builder.HasKey(e => e.EmployeeId);

        // 属性映射
        builder.Property(e => e.EmployeeId)
            .HasColumnName("EMPLOYEE_ID")
            .HasPrecision(10);

        builder.Property(e => e.StaffNumber)
            .IsRequired()
            .HasColumnName("STAFF_NUMBER")
            .HasMaxLength(20)
            .IsUnicode(false);

        builder.Property(e => e.Position)
            .IsRequired()
            .HasColumnName("POSITION")
            .HasMaxLength(50)
            .IsUnicode(false);

        builder.Property(e => e.DepartmentName)
            .HasColumnName("DEPARTMENT_NAME")
            .HasMaxLength(50)
            .IsUnicode(false);

        builder.Property(e => e.StaffType)
            .HasColumnName("STAFF_TYPE")
            .HasMaxLength(30)
            .IsUnicode(false);

        builder.Property(e => e.TeamId)
            .HasColumnName("TEAM_ID")
            .HasPrecision(10);

        builder.Property(e => e.HireDate)
            .HasColumnName("HIRE_DATE");

        builder.Property(e => e.EmploymentStatus)
            .IsRequired()
            .HasColumnName("EMPLOYMENT_STATUS")
            .HasMaxLength(30)
            .IsUnicode(false);

        builder.Property(e => e.ManagerId)
            .HasColumnName("MANAGER_ID")
            .HasPrecision(10);

        builder.Property(e => e.Certification)
            .HasColumnName("CERTIFICATION")
            .HasMaxLength(500)
            .IsUnicode(false);

        builder.Property(e => e.ResponsibilityArea)
            .HasColumnName("RESPONSIBILITY_AREA")
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.Property(e => e.CreatedAt)
            .HasColumnName("CREATED_AT");

        builder.Property(e => e.UpdatedAt)
            .HasColumnName("UPDATED_AT");

        // 唯一索引
        builder.HasIndex(e => e.StaffNumber, "EMPLOYEES_STAFF_NUMBER_UQ")
            .IsUnique();

        // 常规索引
        builder.HasIndex(e => e.DepartmentName, "EMPLOYEES_DEPARTMENT_NAME_IDX");
        builder.HasIndex(e => e.EmploymentStatus, "EMPLOYEES_EMPLOYMENT_STATUS_IDX");
        builder.HasIndex(e => e.ManagerId, "EMPLOYEES_MANAGER_ID_IDX");
        builder.HasIndex(e => e.StaffType, "EMPLOYEES_STAFF_TYPE_IDX");
        builder.HasIndex(e => e.TeamId, "EMPLOYEES_TEAM_ID_IDX");

        // 关系配置
        // 1:1 与 User 的关系
        builder.HasOne(e => e.EmployeeNavigation)
            .WithOne()
            .HasForeignKey<Employee>(e => e.EmployeeId);

        // 自引用关系 (经理)
        builder.HasOne(e => e.Manager)
            .WithMany(e => e.InverseManager)
            .HasForeignKey(e => e.ManagerId);

        // 与团队的归属关系
        builder.HasOne(e => e.Team)
            .WithMany(t => t.Employees)
            .HasForeignKey(e => e.TeamId);

        // 1:N 关系集合配置
        builder.HasMany(e => e.AmusementRides)
            .WithOne(a => a.Manager)
            .HasForeignKey(a => a.ManagerId);

        builder.HasMany(e => e.Attendances)
            .WithOne(a => a.Employee)
            .HasForeignKey(a => a.EmployeeId);

        builder.HasMany(e => e.MaintenanceRecords)
            .WithOne(m => m.Manager)
            .HasForeignKey(m => m.ManagerId);

        builder.HasMany(e => e.PriceHistories)
            .WithOne(p => p.Employee)
            .HasForeignKey(p => p.EmployeeId);

        builder.HasMany(e => e.PriceRules)
            .WithOne(p => p.CreatedByNavigation)
            .HasForeignKey(p => p.CreatedBy);

        builder.HasMany(e => e.Promotions)
            .WithOne(p => p.Employee)
            .HasForeignKey(p => p.EmployeeId);

        builder.HasMany(e => e.RefundRecords)
            .WithOne(r => r.Processor)
            .HasForeignKey(r => r.ProcessorId);

        builder.HasMany(e => e.SalaryRecords)
            .WithOne(s => s.Employee)
            .HasForeignKey(s => s.EmployeeId);

        builder.HasMany(e => e.StaffTeams)
            .WithOne(s => s.Leader)
            .HasForeignKey(s => s.LeaderId);

        builder.HasMany(e => e.TeamMembers)
            .WithOne(t => t.Employee)
            .HasForeignKey(t => t.EmployeeId);

        builder.HasMany(e => e.EmployeeReviewEmployees)
            .WithOne(e => e.Employee)
            .HasForeignKey(e => e.EmployeeId);

        builder.HasMany(e => e.EmployeeReviewEvaluators)
            .WithOne(e => e.Evaluator)
            .HasForeignKey(e => e.EvaluatorId);

        builder.HasMany(e => e.FinancialRecordApprovedByNavigations)
            .WithOne(f => f.ApprovedByNavigation)
            .HasForeignKey(f => f.ApprovedBy);

        builder.HasMany(e => e.FinancialRecordResponsibleEmployees)
            .WithOne(f => f.ResponsibleEmployee)
            .HasForeignKey(f => f.ResponsibleEmployeeId);
    }
}

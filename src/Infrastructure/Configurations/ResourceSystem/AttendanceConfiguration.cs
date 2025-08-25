using DbApp.Domain.Entities.ResourceSystem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DbApp.Infrastructure.Configurations.ResourceSystem;

public class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
{
    public void Configure(EntityTypeBuilder<Attendance> builder)
    {
        // 表名和基础配置
        builder.ToTable("attendances");
        builder.HasKey(a => a.AttendanceId);

        // 属性映射
        builder.Property(a => a.AttendanceId)
            .HasColumnName("attendance_id")
            .HasColumnType("NUMBER(10)");

        builder.Property(a => a.EmployeeId)
            .HasColumnName("employee_id")
            .HasColumnType("NUMBER(10)");

        builder.Property(a => a.AttendanceDate)
            .HasColumnName("attendance_date")
            .HasColumnType("TIMESTAMP");

        builder.Property(a => a.CheckInTime)
            .HasColumnName("check_in_time")
            .HasColumnType("TIMESTAMP");

        builder.Property(a => a.CheckOutTime)
            .HasColumnName("check_out_time")
            .HasColumnType("TIMESTAMP");

        builder.Property(a => a.AttendanceStatus)
            .IsRequired()
            .HasColumnName("attendance_status");

        builder.Property(a => a.LeaveType)
            .HasColumnName("leave_type");

        builder.Property(a => a.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("TIMESTAMP")
            .HasDefaultValueSql("SYSTIMESTAMP");

        builder.Property(a => a.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("TIMESTAMP")
            .HasDefaultValueSql("SYSTIMESTAMP");

        // 索引配置
        builder.HasIndex(a => a.AttendanceDate);
        builder.HasIndex(a => a.AttendanceStatus);
        builder.HasIndex(a => a.EmployeeId);

        // 关系配置
        builder.HasOne(a => a.Employee)
            .WithMany(e => e.Attendances)
            .HasForeignKey(a => a.EmployeeId)
            .IsRequired();
    }
}

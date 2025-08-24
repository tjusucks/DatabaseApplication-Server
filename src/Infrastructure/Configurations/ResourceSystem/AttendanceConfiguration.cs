using DbApp.Domain.Entities.ResourceSystem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DbApp.Infrastructure.Configurations.ResourceSystem;

public class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
{
    public void Configure(EntityTypeBuilder<Attendance> builder)
    {
        // 表名和基础配置
        builder.ToTable("ATTENDANCES");
        builder.HasKey(a => a.AttendanceId);

        // 属性映射
        builder.Property(a => a.AttendanceId)
            .HasColumnName("ATTENDANCE_ID")
            .HasPrecision(10);

        builder.Property(a => a.EmployeeId)
            .HasColumnName("EMPLOYEE_ID")
            .HasPrecision(10);

        builder.Property(a => a.AttendanceDate)
            .HasColumnName("ATTENDANCE_DATE");

        builder.Property(a => a.CheckInTime)
            .HasColumnName("CHECK_IN_TIME");

        builder.Property(a => a.CheckOutTime)
            .HasColumnName("CHECK_OUT_TIME");

        builder.Property(a => a.AttendanceStatus)
            .IsRequired()
            .HasColumnName("ATTENDANCE_STATUS")
            .HasMaxLength(30)
            .IsUnicode(false)
            .HasConversion<string>();

        builder.Property(a => a.LeaveType)
            .HasColumnName("LEAVE_TYPE")
            .HasMaxLength(30)
            .IsUnicode(false)
            .HasConversion<string>();

        builder.Property(a => a.CreatedAt)
            .HasColumnName("CREATED_AT");

        builder.Property(a => a.UpdatedAt)
            .HasColumnName("UPDATED_AT");

        // 索引配置
        builder.HasIndex(a => a.AttendanceDate, "ATTENDANCES_ATTENDANCE_DATE_IDX");
        builder.HasIndex(a => a.AttendanceStatus, "ATTENDANCES_ATTENDANCE_STATUS_IDX");
        builder.HasIndex(a => a.EmployeeId, "ATTENDANCES_EMPLOYEE_ID_IDX");

        // 关系配置
        //builder.HasOne(a => a.Employee)
        //.WithMany(e => e.Attendances)
        //.HasForeignKey(a => a.EmployeeId)
        //.IsRequired();
    }
}

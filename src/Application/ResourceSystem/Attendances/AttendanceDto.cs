using System;
using DbApp.Domain.Enums.ResourceSystem;

namespace DbApp.Application.ResourceSystem.Attendances
{
    public class AttendanceDto
    {
        public int AttendanceId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime AttendanceDate { get; set; }
        public DateTime CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public AttendanceStatus AttendanceStatus { get; set; }
        public LeaveType? LeaveType { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Employee information (simplified to avoid circular references)
        public EmployeeInfoDto Employee { get; set; } = new EmployeeInfoDto();
    }

    public class EmployeeInfoDto
    {
        public int EmployeeId { get; set; }
        public string StaffNumber { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string? DepartmentName { get; set; }
    }
}

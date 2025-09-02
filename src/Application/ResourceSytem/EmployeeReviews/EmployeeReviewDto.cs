namespace DbApp.Application.ResourceSystem.EmployeeReviews
{
    public class EmployeeReviewDto
    {
        public int ReviewId { get; set; }
        public int EmployeeId { get; set; }
        public string Period { get; set; } = string.Empty;
        public decimal Score { get; set; }
        public string? EvaluationLevel { get; set; }
        public int? EvaluatorId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public EmployeeSimpleDto Employee { get; set; } = new EmployeeSimpleDto();
    }

    public class EmployeeSimpleDto
    {
        public int EmployeeId { get; set; }
        public string StaffNumber { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
    }
}

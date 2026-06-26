namespace LeaveManagement.API.DTOs
{
    public class LeaveRequestDto
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string FromDate { get; set; } = string.Empty;
        public string ToDate { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? Remarks { get; set; }
        public string CreatedAt { get; set; } = string.Empty;
    }

    public class CreateLeaveRequestDto
    {
        public string FromDate { get; set; } = string.Empty;
        public string ToDate { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
    }

    public class ReviewLeaveDto
    {
        public string Status { get; set; } = string.Empty;
        public string? Remarks { get; set; }
    }
}

namespace AttendanceSystem.ViewModel.WebAPI
{
    public class DashboardCountVM
    {
        public decimal TotalAttendance { get; set; } 
        public int LeavePendingForApprove { get; set; }
        public decimal PendingSalary { get; set; }
        public double thisMonthHoliday { get; set; }
        public string LastMonthRating { get; set; }
        public int AttendancePendingForApprove { get; set; }
    }
}
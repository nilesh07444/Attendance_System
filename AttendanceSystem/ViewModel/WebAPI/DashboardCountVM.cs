namespace AttendanceSystem.ViewModel.WebAPI
{
    public class DashboardCountVM
    {
        public int TotalAttendance { get; set; }
        public int TotalAbsent { get; set; }
        public int LeavePendingForApprove { get; set; }
        public long PendingSalary { get; set; }
        public double thisMonthHoliday { get; set; }
        public string LastMonthRating { get; set; }
    }
}

namespace AttendanceSystem.ViewModel.WebAPI.ViewModel
{
	public class UserTokenVM
	{
		public long EmployeeId { get; set; }
		public string EmployeeCode { get; set; }
		public int RoleId { get; set; }
		public string UserName { get; set; }
		public long CompanyId { get; set; }
		public long CompanyTypeId { get; set; }
		public bool IsTrailMode { get; set; }
    }
}

using System;

namespace AttendanceSystem.ViewModel.WebAPI.ViewModel
{
	public class JWTAccessTokenVM
	{
		public string Token { get; set; }
		public int ValidityInMin { get; set; }
		public DateTime ExpiresOnUTC { get; set; }
	}
}

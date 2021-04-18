// File name:		BaseUserController
// Target:			Base api for request/controllers that require user authentication
// Package:	 		ResourcePlanner.API
// Last changes:    Dipak Jumde
// Date				15-11-2018

// ============================================================================= 
//  History details:
// 
//  Version         Date                Status          LastChangedBy
// -----------------------------------------------------------------------------
// 1.0             04-04-2021           Created         Dipak Jumde
// =============================================================================

using AttendanceSystem.Filters.JWT;
using AttendanceSystem.ViewModel.WebAPI.ViewModel;
using Newtonsoft.Json;
using System.Web;

namespace AttendanceSystem.Areas.WebAPI.Controllers
{
    [JwtAuthentication]
	public class BaseUserController : BaseController
    {
		public BaseUserController()
		{
			initilizeUserTokeInfo();
		}

		private void initilizeUserTokeInfo()
		{
			string userData;
			string errorMessage;

			string token = string.Empty;
			try
			{
				token = HttpContext.Current.Request.Headers.Get("Authorization");
			}
			catch
			{
			}

			if (token == null || token.Substring(0, 6) != "Bearer")
			{
				return;
			}

			token = token.Substring(7);

			if (string.IsNullOrEmpty(token))
			{
				return;
			}


			if (JwtAuthenticationAttribute.ValidateToken(token, out userData, out errorMessage))
			{
				if (userData == null)
				{
					return;
				}

				try
				{
					base.UTI = JsonConvert.DeserializeObject<UserTokenVM>(userData);
				}
				catch
				{
					base.UTI = null;
					// The HttpRequestMessage of the current System.Web.Http.ApiController.
					// throw new  HttpResponseException("Customer does not have any account", HttpStatusCode.BadRequest)
					// BadRequest("Invalid request...");
					return;
				}

			}
			else
			{
				return;
			}
		}

	}
}

using AttendanceSystem.Helper;
using MyMobileApp.Helper;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace AttendanceSystem.Filters.JWT
{
    public class JwtAuthenticationAttribute : Attribute, IAuthenticationFilter
	{
		public string Realm { get; set; }
		public bool AllowMultiple => false;
		
		public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
		{
			var request = context.Request;
			var authorization = request.Headers.Authorization;

			if (authorization == null || authorization.Scheme == null || authorization.Scheme.ToLower() != "bearer")
			{
				context.ErrorResult = new AuthenticationFailureResult(ErrorMessage.AuthorizationTokenMissing, request);
				return;
			}

			if (string.IsNullOrEmpty(authorization.Parameter))
			{
				context.ErrorResult = new AuthenticationFailureResult(ErrorMessage.AuthorizationTokenMissing, request);
				return;
			}

			await Task.Run(() => 
			{
				try
				{
					var token = authorization.Parameter;
					string errorMessage;
					var principal = AuthenticateJwtToken(token, out errorMessage);
					if (principal == null)
						context.ErrorResult = new AuthenticationFailureResult(errorMessage, request);
					else
						context.Principal = principal;
				}
				catch (Exception ex)
				{
					//ErrorLogHelper.LogSystemError(ex.Message);
					context.ActionContext.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
					context.ActionContext.Response.Content = new StringContent(ErrorMessage.InternalServerError);
				}
			});
		}
		
		protected IPrincipal AuthenticateJwtToken(string token, out string errorMessage)
		{
			string userData;
			if (ValidateToken(token, out userData, out errorMessage))
			{
				// based on username to get more information from database in order to build local identity
				var claims = new List<Claim>
				{
					new Claim(ClaimTypes.UserData, userData)
                };

				var identity = new ClaimsIdentity(claims, "JWT");
				IPrincipal user = new ClaimsPrincipal(identity);

				return user;
			}

			return null;
		}


		public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
		{
			Challenge(context);
			return Task.FromResult(0);
		}


		public static bool ValidateToken(string token, out string userData, out string errorMessage)
		{
			userData = null;

			var simplePrinciple = JWTAuthenticationHelper.GetPrincipal(token, out errorMessage);

			if (simplePrinciple == null)
				return false;

			var identity = simplePrinciple.Identity as ClaimsIdentity;
			if (identity == null)
				return false;

			if (!identity.IsAuthenticated)
				return false;

			var userDataClaim = identity.FindFirst(ClaimTypes.UserData);
			userData = userDataClaim?.Value;

			if (string.IsNullOrEmpty(userData))
				return false;
			
			return true;
		}
		
		private void Challenge(HttpAuthenticationChallengeContext context)
		{
			string parameter = null;

			if (!string.IsNullOrEmpty(Realm))
				parameter = "realm=\"" + Realm + "\"";

			context.ChallengeWith("Bearer", parameter);
		}
	}
}
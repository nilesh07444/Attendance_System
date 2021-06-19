using AttendanceSystem.ViewModel.WebAPI.ViewModel;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Web;

namespace AttendanceSystem.Helper
{
    public static class JWTAuthenticationHelper
	{
		public static JWTAccessTokenVM GenerateToken(UserTokenVM utiVM) 
		{
			string symmetricKey = ConfigurationManager.AppSettings["JWT_Symmetric_Key"]; 
			byte[] symmetricKeyBytes = Convert.FromBase64String(symmetricKey);
			JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
			
			DateTime tokenGeneratedOnUTC = CommonMethod.CurrentIndianDateTime();
			int tokenValidityInMins =Convert.ToInt32(ConfigurationManager.AppSettings["JWT_Validity_Mins"]);
			DateTime tokenExpiresOnUTC = tokenGeneratedOnUTC.AddMinutes(tokenValidityInMins);
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new[]
				{
					new Claim(ClaimTypes.UserData, JsonConvert.SerializeObject(utiVM))			
				}),
				IssuedAt = tokenGeneratedOnUTC,
				Expires = tokenExpiresOnUTC,
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKeyBytes), SecurityAlgorithms.HmacSha256Signature)
			};

			var stoken = tokenHandler.CreateToken(tokenDescriptor);

			return new JWTAccessTokenVM()
			{
				Token = tokenHandler.WriteToken(stoken),
                ValidityInMin = tokenValidityInMins,
				ExpiresOnUTC = tokenExpiresOnUTC
			};
		}


		public static ClaimsPrincipal GetPrincipal(string token, out string errorMessage)
		{
			errorMessage = string.Empty;

			try
			{
				var tokenHandler = new JwtSecurityTokenHandler();
				var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

				if (jwtToken == null)
					return null;

				string symmetricKey = ConfigurationManager.AppSettings["JWT_Symmetric_Key"];
				byte[] symmetricKeyBytes = Convert.FromBase64String(symmetricKey);

				var validationParameters = new TokenValidationParameters()
				{
					IssuerSigningKey = new SymmetricSecurityKey(symmetricKeyBytes),
					RequireExpirationTime = true,
					ValidateIssuer = false,
					ValidateAudience = false,
					LifetimeValidator = LifetimeValidator
				};

				SecurityToken securityToken;
				var principal = tokenHandler.ValidateToken(token, validationParameters, out securityToken);
				
				return principal;
			}
			catch (SecurityTokenInvalidLifetimeException)
			{
				errorMessage = ErrorMessage.TokenExpired;
				return null;
			}
			catch (ArgumentException)
			{
				errorMessage = ErrorMessage.InvalidToken;

                return null;
			}
			catch (SecurityTokenInvalidSignatureException)
			{
				errorMessage = ErrorMessage.InvalidToken;
				return null;
			}
			catch (Exception)
			{
				throw;
			}
		}


		public static UserTokenVM GetContextUserInfoVM() // this HttpAuthenticationChallengeContext context)
		{
			var claimPrincipal = (HttpContext.Current.User as ClaimsPrincipal);
			if (!claimPrincipal.Identity.IsAuthenticated)
				return null;
			var userData = (HttpContext.Current.User as ClaimsPrincipal).FindFirst(ClaimTypes.UserData).Value;
			return JsonConvert.DeserializeObject<UserTokenVM>(userData);
		}

		static bool LifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters validationParameters)
		{
			return (expires != null && CommonMethod.CurrentIndianDateTime() < expires);
		}
	}
}
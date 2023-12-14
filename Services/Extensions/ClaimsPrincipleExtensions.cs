using System.Security.Claims;

namespace Services.Extensions
{
	public static class ClaimsPrincipleExtensions
	{
		/// <summary>
		/// Get id from token
		/// </summary>
		/// <param name="principal"></param>
		/// <returns></returns>
		public static string GetId(this ClaimsPrincipal principal)
		{
			var userIdClaim = principal.FindFirst(c => c.Type == "Id");
			if (userIdClaim != null && !string.IsNullOrEmpty(userIdClaim.Value))
			{
				return userIdClaim.Value;
			}

			return null;
		}

		/// <summary>
		/// Get roles from token
		/// </summary>
		/// <param name="principal"></param>
		/// <returns></returns>
		public static string GetRoles(this ClaimsPrincipal principal)
		{
			var userRoleClaim = principal.FindFirst(c => c.Type == ClaimTypes.Role) ?? principal.FindFirst(c => c.Type == "Role");
			if (userRoleClaim != null && !string.IsNullOrEmpty(userRoleClaim.Value))
			{
				return userRoleClaim.Value;
			}

			return null;
		}
	}
}
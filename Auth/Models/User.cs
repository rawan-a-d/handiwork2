using Microsoft.AspNetCore.Identity;

namespace Auth.Models
{
	/// <summary>
	/// This class is used to case the primary key to integer
	/// </summary>
	public class User : IdentityUser<int>
	{
		// custom properties
		public string Name { get; set; }
	}
}
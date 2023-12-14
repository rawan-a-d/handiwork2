using Microsoft.AspNetCore.Identity;

namespace Auth.Models
{
	public class Role : IdentityRole<int>
	{
		public Role(string name) : base(name)
		{

		}
	}
}
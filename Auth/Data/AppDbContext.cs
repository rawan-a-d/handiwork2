using Auth.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Auth.Data
{
	/// <summary>
	/// Database setup
	/// we will use custom User and Role classes and the primary key is an int
	/// </summary>
	public class AppDbContext : IdentityDbContext<User, Role, int>
	{
		public AppDbContext(DbContextOptions<AppDbContext> options)
			: base(options)
		{

		}
	}
}
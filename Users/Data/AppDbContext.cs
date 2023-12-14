using Microsoft.EntityFrameworkCore;
using Users.Models;

namespace Users.Data
{
	// Data layer
	// used to define db tables
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt)
		{

		}

		public DbSet<User> Users { get; set; }
	}
}
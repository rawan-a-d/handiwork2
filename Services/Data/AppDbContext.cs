using Microsoft.EntityFrameworkCore;
using Services.Models;

namespace Services.Data
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt)
		{

		}

		public DbSet<Service> Services { get; set; }
		public DbSet<ServiceCategory> ServiceCategories { get; set; }
		public DbSet<User> Users { get; set; }

		/// <summary>
		/// Represents a table in the db called Photos
		/// </summary>
		/// <value></value>
		public DbSet<Photo> Photos { get; set; }

		/// <summary>
		/// This method is needed to setup the many to many relationship
		/// </summary>
		/// <param name="builder"></param>
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// User
			modelBuilder.Entity<User>()
				.HasMany(u => u.Services)
				.WithOne(s => s.User)
				.HasForeignKey(s => s.UserId)
				.OnDelete(DeleteBehavior.Cascade);

			// Service
			modelBuilder.Entity<Service>()
				.HasOne(s => s.User)
				.WithMany(u => u.Services)
				.HasForeignKey(s => s.UserId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder
				.Entity<Service>()
				.HasOne(s => s.ServiceCategory)
				.WithMany(u => u.Services)
				.HasForeignKey(s => s.ServiceCategoryId);

			modelBuilder
				.Entity<Service>()
				.HasMany(s => s.Photos)
				.WithOne(p => p.Service)
				.HasForeignKey(p => p.ServiceId);

			// Service category
			modelBuilder
				.Entity<ServiceCategory>()
				.HasMany(sc => sc.Services)
				.WithOne(s => s.ServiceCategory)
				.HasForeignKey(s => s.ServiceCategoryId);

			// Photo
			modelBuilder
				.Entity<Photo>()
				.HasOne(p => p.Service)
				.WithMany(s => s.Photos)
				.HasForeignKey(p => p.ServiceId);
		}
	}
}
using Auth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Auth.Data
{
	public static class PrepDb
	{
		public async static void PrepPopulation(IApplicationBuilder app)
		{
			using (var serviceScope = app.ApplicationServices.CreateScope())
			{
				await SeedData(
					serviceScope.ServiceProvider.GetService<UserManager<User>>(),
					serviceScope.ServiceProvider.GetService<RoleManager<Role>>()
				);
			}
		}

		private static async Task SeedData(UserManager<User> userManager, RoleManager<Role> roleManager)
		{
			// check if users table contains any users
			if (await userManager.Users.AnyAsync())
			{
				return;
			}

			// roles
			var roles = new List<Role> {
				new Role("User"),
				new Role("Admin"),
				new Role("Moderator")
			};
			// create roles
			foreach (var role in roles)
			{
				await roleManager.CreateAsync(role);
			}

			// Create an admin
			var admin = new User
			{
				Email = "admin@admin.com",
				UserName = "admin",
				Name = "Admin"
			};
			await userManager.CreateAsync(admin, "Pa$$w0rd");
			await userManager.AddToRolesAsync(admin, new[] { "Admin", "Moderator" });

			// Create other users
			var rawan = new User
			{
				Email = "rawan@gmail.com",
				UserName = "Rawan",
				Name = "Rawan"
			};
			await userManager.CreateAsync(rawan, "Pa$$w0rd");
			await userManager.AddToRolesAsync(rawan, new[] { "User" });

			var omar = new User
			{
				Email = "omar@gmail.com",
				UserName = "Omar",
				Name = "Omar"
			};
			await userManager.CreateAsync(omar, "Pa$$w0rd");
			await userManager.AddToRolesAsync(omar, new[] { "User" });

			var ranim = new User
			{
				Email = "ranim@gmail.com",
				UserName = "Ranim",
				Name = "Ranim"
			};
			await userManager.CreateAsync(ranim, "Pa$$w0rd");
			await userManager.AddToRolesAsync(ranim, new[] { "User" });
		}
	}
}
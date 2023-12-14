using Services.Data;
using Services.Models;

namespace Users.Data
{
	public static class PrepDb
	{
		public static void PrepPopulation(IApplicationBuilder app)
		{
			using (var serviceScope = app.ApplicationServices.CreateScope())
			{
				SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>());
			}
		}

		private static void SeedData(AppDbContext context)
		{
			// if no data
			if (!context.Users.Any() && !context.ServiceCategories.Any() && !context.Services.Any())
			{
				Console.WriteLine("--> Seeding data...");

				context.Users.AddRange(
					new User() { Name = "Admin", ExternalId = 1 },
					new User() { Name = "Rawan", ExternalId = 2 },
					new User() { Name = "Omar", ExternalId = 3 },
					new User() { Name = "Ranim", ExternalId = 4 }
				);

				context.ServiceCategories.AddRange(
					new ServiceCategory() { Name = "Painting" },
					new ServiceCategory() { Name = "Install Laminaat" },
					new ServiceCategory() { Name = "Plumbing" }
				);

				context.Services.AddRange(
					new Service() { Info = "I paint....", UserId = 2, ServiceCategoryId = 1 },
					new Service() { Info = "I install laminaat", UserId = 2, ServiceCategoryId = 2 },
					new Service() { Info = "I am a plumber", UserId = 3, ServiceCategoryId = 3 }
				);

				context.SaveChanges();
			}
			else
			{
				Console.WriteLine("--> We already have data");
			}
		}
	}
}
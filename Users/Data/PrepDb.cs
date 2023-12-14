using Users.Models;
using Microsoft.EntityFrameworkCore;

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
			if (!context.Users.Any())
			{
				Console.WriteLine("--> Seeding data...");

				context.Users.AddRange(
					new User() { ExternalId = 1, Name = "Admin", Email = "admin@admin.com", Phone = "081234566", Address = "4543 YH 2", About = "Handiwork admin" },
					new User() { ExternalId = 2, Name = "Rawan", Email = "rawan@gmail.com", Phone = "087667283", Address = "3496 WJ 100", About = "A professional painter experienced in in painting outdoor and indoor surfaces made of all materials such as wood, concrete, or brick." },
					new User() { ExternalId = 3, Name = "Omar", Email = "omar@gmail.com", Phone = "087667283", Address = "4783 GW 138", About = "A well versed plumber with over 30 years of experience and the proven ability to offer technical direction in the engineering and maintenance of plumbing systems across commercial and residential buildings. I am efficient in removal of old paint, graffiti, or other markings and capable of performing minor repairs such as filling holes or replacing wood." },
					new User() { ExternalId = 4, Name = "Ranim", Email = "ranim@gmail.com", Phone = "087667283", Address = "2395 WQ 45" }
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
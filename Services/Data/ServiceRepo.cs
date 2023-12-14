using Microsoft.EntityFrameworkCore;
using Services.Models;
using System.Linq;

namespace Services.Data
{
	public class ServiceRepo : IServiceRepo
	{
		private readonly AppDbContext _context;
		public ServiceRepo(AppDbContext context)
		{
			_context = context;
		}

		/// <summary>
		/// Create a new service
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="service"></param>
		public void CreateService(int userId, Service service)
		{
			if (service == null)
			{
				throw new ArgumentNullException(nameof(service));
			}

			// add userId to service object
			service.UserId = userId;

			_context.Services.Add(service);
		}

		/// <summary>
		/// Get services owned by a specific user
		/// </summary>
		/// <param name="userId">the owner</param>
		/// <returns></returns>
		public IEnumerable<Service> GetServicesForUser(int userId)
		{
			return _context.Services
				.Where(s => s.UserId == userId)
				.Include(sc => sc.ServiceCategory)
				.Include(p => p.Photos)
				.ToList();
		}

		/// <summary>
		/// Get service by its id, the user and its category
		/// </summary>
		/// <param name="serviceId"></param>
		/// <param name="serviceCategoryId"></param>
		/// <param name="userId"></param>
		/// <returns></returns>
		public Service GetService(int serviceId, int userId)
		{
			return _context.Services
				.Where(s => s.Id == serviceId && s.UserId == userId)
				.Include(p => p.Photos)
				.Include(sc => sc.ServiceCategory)
				.FirstOrDefault();
		}

		/// <summary>
		/// Update service
		/// </summary>
		/// <param name="service"></param>
		public void UpdateService(Service service)
		{
			// Add flag to the entity that has been modified
			_context.Entry(service).State = EntityState.Modified;
		}

		/// <summary>
		/// Delete service
		/// </summary>
		/// <param name="service"></param>
		public void DeleteService(Service service)
		{
			_context.Services.Remove(service);
		}


		// search for a category (laminaat) and find users who have services
		public IEnumerable<User> SearchService(string serviceCategoryName)
		{
			ServiceCategory category = _context.ServiceCategories.Where(s => s.Name == serviceCategoryName).FirstOrDefault();

			//context.Customers
			//    .Include(c => c.Orders.Where(o => o.Name != "Foo")).ThenInclude(o => o.OrderDetails)
			//    .Include(c => c.Orders).ThenInclude(o => o.Customer)
			return _context.Users
				//.Include(s => s.Services)
				.Where(u => u.Services.Any(s => s.ServiceCategory.Name.ToLower().Contains(serviceCategoryName.ToLower())))
				.Distinct()
				.ToList();
		}

		/// <summary>
		/// Persist changes to the database
		/// </summary>
		/// <returns></returns>
		public bool SaveChanges()
		{
			return (_context.SaveChanges() >= 0);
		}
	}
}
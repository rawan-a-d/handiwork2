using Microsoft.EntityFrameworkCore;
using Services.Models;

namespace Services.Data
{
	public class ServiceCategoryRepo : IServiceCategoryRepo
	{
		private readonly AppDbContext _context;
		public ServiceCategoryRepo(AppDbContext context)
		{
			_context = context;
		}

		public void CreateServiceCategory(ServiceCategory serviceCategory)
		{
			_context.ServiceCategories.Add(serviceCategory);
		}

		public IEnumerable<ServiceCategory> GetServiceCategories()
		{
			return _context.ServiceCategories;
		}

		public ServiceCategory GetServiceCategory(int id)
		{
			return _context.ServiceCategories
				.Where(sc => sc.Id == id)
				.FirstOrDefault();
		}

		public void UpdateServiceCategory(ServiceCategory serviceCategory)
		{
			// Add flag to the entity that has been modified
			_context.Entry(serviceCategory).State = EntityState.Modified;
		}

		public void DeleteServiceCategory(ServiceCategory serviceCategory)
		{
			_context.ServiceCategories.Remove(serviceCategory);
		}

		public bool DoesServiceCategoryExist(string name)
		{
			return (_context.ServiceCategories.FirstOrDefault(sc => sc.Name == name) != null);
		}

		public bool SaveChanges()
		{
			return (_context.SaveChanges() >= 0);
		}
	}
}
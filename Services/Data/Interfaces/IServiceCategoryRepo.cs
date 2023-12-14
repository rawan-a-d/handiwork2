using Services.Models;

namespace Services.Data
{
	public interface IServiceCategoryRepo
	{
		void CreateServiceCategory(ServiceCategory serviceCategory);

		IEnumerable<ServiceCategory> GetServiceCategories();

		ServiceCategory GetServiceCategory(int id);

		void UpdateServiceCategory(ServiceCategory serviceCategory);

		void DeleteServiceCategory(ServiceCategory serviceCategory);

		public bool DoesServiceCategoryExist(string name);

		bool SaveChanges();
	}
}
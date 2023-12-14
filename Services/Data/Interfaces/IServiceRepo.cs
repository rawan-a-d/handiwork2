using Services.Models;

namespace Services.Data
{
	public interface IServiceRepo
	{
		bool SaveChanges();

		IEnumerable<Service> GetServicesForUser(int userId);

		Service GetService(int serviceId, int userId);

		void CreateService(int userId, Service service);

		void UpdateService(Service service);

		void DeleteService(Service service);

		IEnumerable<User> SearchService(string serviceCategoryName);
	}
}
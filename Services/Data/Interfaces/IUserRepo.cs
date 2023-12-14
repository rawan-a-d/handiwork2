using Services.Models;

namespace Services.Data
{
	public interface IUserRepo
	{
		public void CreateUser(User user);

		User GetUser(int id);

		User GetUserByExternalId(int externalId);

		void UpdateUser(User user);

		void DeleteUser(User user);

		bool ExternalUserExists(int externalPlatformId);

		bool SaveChanges();
	}
}
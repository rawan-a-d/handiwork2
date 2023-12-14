using Users.Models;

namespace Users.Data
{
	// Database handler for user operations
	public class UserRepo : IUserRepo
	{
		private readonly AppDbContext _context;

		public UserRepo(AppDbContext context)
		{
			_context = context;
		}

		public void UpdateUser(User user)
		{
			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}

			_context.Users.Update(user);
		}

		public User GetUser(int id)
		{
			return _context.Users.FirstOrDefault(u => u.Id == id);
		}

		public IEnumerable<User> GetUsers()
		{
			return _context.Users;
		}

		public void DeleteUser(User user)
		{
			_context.Users.Remove(user);
		}

		public Boolean UserExists(int id)
		{
			return _context.Users.Any(u => u.Id == id);
		}

		public bool SaveChanges()
		{
			return (_context.SaveChanges() >= 0);
		}
	}
}
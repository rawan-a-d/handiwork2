using Microsoft.EntityFrameworkCore;
using Services.Models;

namespace Services.Data
{
	public class UserRepo : IUserRepo
	{
		private readonly AppDbContext _context;
		public UserRepo(AppDbContext context)
		{
			_context = context;
		}

		public void CreateUser(User user)
		{
			_context.Users.Add(user);
		}

		public User GetUser(int id)
		{
			return _context.Users.Find(id);
		}

		public User GetUserByExternalId(int externalId)
		{
			return _context.Users.FirstOrDefault(u => u.ExternalId == externalId);
		}

		public void UpdateUser(User user)
		{
			_context.Entry(user).State = EntityState.Modified;
		}

		public void DeleteUser(User user)
		{
			_context.Users.Remove(user);
		}

		public bool ExternalUserExists(int externalId)
		{
			return _context.Users.Any(p => p.ExternalId == externalId);
		}

		public bool SaveChanges()
		{
			return (_context.SaveChanges() >= 0);
		}
	}
}
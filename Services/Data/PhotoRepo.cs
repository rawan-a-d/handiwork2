using Services.Models;

namespace Services.Data
{
	public class PhotoRepo : IPhotoRepo
	{
		private readonly AppDbContext _context;

		public PhotoRepo(AppDbContext context)
		{
			_context = context;
		}

		public async void CreatePhoto(Photo photo)
		{
			await _context.Photos.AddAsync(photo);
		}

		/// <summary>
		/// Get photo by id
		/// </summary>
		/// <param name="id">the id</param>
		/// <returns>the photo</returns>
		public async Task<Photo> GetPhotoById(int id)
		{
			return await _context.Photos.FindAsync(id);
		}

		/// <summary>
		/// Remove photo
		/// </summary>
		/// <param name="photo">the photo</param>
		public void DeletePhoto(Photo photo)
		{
			_context.Photos.Remove(photo);
		}

		public bool SaveChanges()
		{
			return _context.SaveChanges() >= 0;
		}
	}
}
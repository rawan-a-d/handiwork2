using Services.Models;

namespace Services.Data
{
	public interface IPhotoRepo
	{
		bool SaveChanges();

		void CreatePhoto(Photo photo);

		Task<Photo> GetPhotoById(int id);

		void DeletePhoto(Photo photo);
	}
}
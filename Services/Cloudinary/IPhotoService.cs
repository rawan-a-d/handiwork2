using CloudinaryDotNet.Actions;

namespace Services.Data
{
	/// <summary>
	/// Interface for uploading and deleting photos from Cloudinary
	/// </summary>
	public interface IPhotoService
	{
		Task<ImageUploadResult> AddPhotoAsync(IFormFile file);
		Task<DeletionResult> DeletePhotoAsync(string publicId);
	}
}
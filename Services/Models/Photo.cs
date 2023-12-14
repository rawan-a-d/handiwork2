namespace Services.Models
{
	// Create a table in the db called Photots
	// without adding it in the DataContext because we don't need to get individual photos, we need them only in connection with users
	public class Photo
	{
		public int Id { get; set; }

		public string Url { get; set; }

		// Cloudinary id, used when deleting
		public string PublicId { get; set; }

		public int ServiceId { get; set; }

		// navigation properties
		public Service Service { get; set; }
	}
}
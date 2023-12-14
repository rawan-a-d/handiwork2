using System.ComponentModel.DataAnnotations;

namespace Services.Models
{
	public class User
	{
		[Key]
		[Required]
		public int Id { get; set; }

		[Required]
		public int ExternalId { get; set; }

		[Required]
		public string Name { get; set; }

		// navigation properties
		public ICollection<Service> Services { get; set; }
	}
}
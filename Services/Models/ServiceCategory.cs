using System.ComponentModel.DataAnnotations;

namespace Services.Models
{
	public class ServiceCategory
	{
		[Key]
		[Required]
		public int Id { get; set; }

		[Required]
		public string Name { get; set; }

		// navigation properties
		public ICollection<Service> Services { get; set; }
	}
}
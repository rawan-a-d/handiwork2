using System.ComponentModel.DataAnnotations;

namespace Users.Models
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

		[Required]
		public string Email { get; set; }

		public string Phone { get; set; }

		public string Address { get; set; }

		public string About { get; set; }
	}
}

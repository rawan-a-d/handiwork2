using System.ComponentModel.DataAnnotations;

namespace Services.Dtos
{
	public class ServiceCreateDto
	{
		[Required]
		// Allow up to 200 uppercase and lowercase characters. Use custom error.
		[RegularExpression(@"^[a-zA-Z''-,.'\s]{1,200}$",
			ErrorMessage = "Length must be less than 200 and only letters and the following characters (.,') are allowed")
		]
		public string Info { get; set; }

		[Required]
		public int ServiceCategoryId { get; set; }
	}
}
using System.ComponentModel.DataAnnotations;

namespace Services.Dtos
{
	public class ServiceCategoryCreateDto
	{
		[Required]
		// Allow up to 200 uppercase and lowercase characters. Use custom error.
		[RegularExpression(@"^[a-zA-Z'\s]{1,100}$",
			ErrorMessage = "Length must be less than 100 and only letters, spaces and apostrophes are allowed")
		]
		public string Name { get; set; }
	}
}
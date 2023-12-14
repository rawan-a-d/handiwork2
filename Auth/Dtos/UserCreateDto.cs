using System.ComponentModel.DataAnnotations;

namespace Auth.Dtos
{
	public class UserCreateDto
	{
		[Required]
		// Allow up to 40 uppercase and lowercase 
		// characters. Use custom error.
		[RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$",
			ErrorMessage = "Characters are not allowed.")
		]
		public string Name { get; set; }

		[Required]
		[RegularExpression(@"^[\w-\.]+@([\w -]+\.)+[\w-]{2,4}$",
			ErrorMessage = "Email is not valid.")
		]
		public string Email { get; set; }

		[Required]
		public string Password { get; set; }
	}
}
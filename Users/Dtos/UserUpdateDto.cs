using System.ComponentModel.DataAnnotations;

namespace Users.Dtos
{
	public class UserUpdateDto
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

		[Phone]
		public string Phone { get; set; }

		// Allow up to 40 uppercase and lowercase, numbers and spaces 
		// characters. Use custom error.
		[RegularExpression(@"^[\w\s]{1,40}$",
			ErrorMessage = "Characters are not allowed.")
		]
		public string Address { get; set; }
	}
}
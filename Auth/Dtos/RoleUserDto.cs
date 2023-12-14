using System.ComponentModel.DataAnnotations;

namespace Auth.Dtos
{
	public class RoleUserDto
	{
		[Required]
		public string Email { get; set; }

		[Required]
		public string RoleName { get; set; }
	}
}
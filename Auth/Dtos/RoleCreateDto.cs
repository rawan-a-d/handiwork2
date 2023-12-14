using System.ComponentModel.DataAnnotations;

namespace Auth.Dtos
{
	public class RoleCreateDto
	{
		[Required]
		public string Name { get; set; }
	}
}
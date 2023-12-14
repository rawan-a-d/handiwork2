using Services.Models;

namespace Services.Dtos
{
	/// <summary>
	/// ServiceReadDto is similar to Service except that:
	/// it makes use of PhotoDto and ServiceCategoryDto so it doesn't cause a circular reference exception
	/// </summary>
	public class ServiceReadDto
	{
		public int Id { get; set; }

		public string Info { get; set; }

		public int UserId { get; set; }

		public int ServiceCategoryId { get; set; }

		public ServiceCategoryReadDto ServiceCategory { get; set; }

		public ICollection<PhotoDto> Photos { get; set; }
	}
}
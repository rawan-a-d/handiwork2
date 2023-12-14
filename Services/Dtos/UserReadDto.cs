namespace Services.Dtos
{
	public class UserReadDto
	{
		public int Id { get; set; }

		public int ExternalId { get; set; }

		public string Name { get; set; }

		// navigation properties
		//public ICollection<ServiceReadDto> Services { get; set; }
	}
}
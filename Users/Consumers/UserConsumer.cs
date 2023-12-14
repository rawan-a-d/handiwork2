using AutoMapper;
using MassTransit;
using MessagingModels;
using Users.Data;
using Users.Models;

namespace Users.Consumers
{
	internal class UserConsumer : IConsumer<UserCreated>
	{
		private readonly ILogger<UserCreated> _logger;
		private readonly AppDbContext _context;
		private readonly IMapper _mapper;

		public UserConsumer(ILogger<UserCreated> logger, AppDbContext context, IMapper mapper)
		{
			_mapper = mapper;
			_context = context;
			_logger = logger;
		}

		public async Task Consume(ConsumeContext<UserCreated> context)
		{
			await Console.Out.WriteLineAsync(context.Message.Name);

			var userCreated = context.Message;
			var userModel = _mapper.Map<User>(userCreated);

			// Add user to db
			await _context.Users.AddAsync(userModel);
			_context.SaveChanges();

			_logger.LogInformation($"Got new message {context.Message.Name}");
		}
	}
}
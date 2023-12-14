using AutoMapper;
using MassTransit;
using MessagingModels;
using Services.Data;
using Services.Models;

namespace Services.Consumers
{
	internal class UserCreatedConsumer : IConsumer<UserCreated>
	{
		private readonly IUserRepo _userRepository;
		private readonly IMapper _mapper;
		private readonly ILogger<UserCreated> _logger;

		public UserCreatedConsumer(IUserRepo userRepository, IMapper mapper, ILogger<UserCreated> logger)
		{
			_userRepository = userRepository;
			_mapper = mapper;
			_logger = logger;
		}

		public async Task Consume(ConsumeContext<UserCreated> context)
		{
			await Console.Out.WriteLineAsync(context.Message.Name);
			_logger.LogInformation($"Got new message {context.Message.Name}");

			UserCreated userCreated = context.Message;

			var userModel = _mapper.Map<User>(userCreated);

			_userRepository.CreateUser(userModel);

			// save to db
			_userRepository.SaveChanges();
		}
	}
}
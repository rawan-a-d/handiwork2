
using AutoMapper;
using MassTransit;
using MessagingModels;
using Services.Data;

namespace Services.Consumers
{
	public class UserDeletedConsumer : IConsumer<UserDeleted>
	{
		private IUserRepo _userRepository;
		private readonly IMapper _mapper;
		private readonly ILogger<UserDeleted> _logger;

		public UserDeletedConsumer(IUserRepo userRepository, IMapper mapper, ILogger<UserDeleted> logger)
		{
			_userRepository = userRepository;
			_mapper = mapper;
			_logger = logger;
		}

		public async Task Consume(ConsumeContext<UserDeleted> context)
		{
			await Console.Out.WriteLineAsync(context.Message.Id.ToString());
			_logger.LogInformation($"Got new message {context.Message.Id}");

			var userDeleted = context.Message;

			var userModel = _userRepository.GetUserByExternalId(userDeleted.Id);

			_userRepository.DeleteUser(userModel);

			// save to db
			_userRepository.SaveChanges();
		}
	}
}
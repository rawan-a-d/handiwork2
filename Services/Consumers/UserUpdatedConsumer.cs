using AutoMapper;
using MassTransit;
using MessagingModels;
using Services.Data;
using Services.Models;

namespace Services.Consumers
{
	internal class UserUpdatedConsumer : IConsumer<UserUpdated>
	{
		private IUserRepo _userRepository;
		private readonly IMapper _mapper;
		private readonly ILogger<UserUpdated> _logger;

		public UserUpdatedConsumer(IUserRepo userRepository, IMapper mapper, ILogger<UserUpdated> logger)
		{
			_userRepository = userRepository;
			_mapper = mapper;
			_logger = logger;
		}

		public async Task Consume(ConsumeContext<UserUpdated> context)
		{
			await Console.Out.WriteLineAsync(context.Message.Id.ToString());
			_logger.LogInformation($"Got new message {context.Message.Id}");

			var userUpdated = context.Message;
			var userModel = _userRepository.GetUserByExternalId(userUpdated.Id);

			// map UserUpdated to User
			var mappedUser = _mapper.Map(userUpdated, userModel);

			_userRepository.UpdateUser(mappedUser);

			// save to db
			_userRepository.SaveChanges();
		}
	}
}
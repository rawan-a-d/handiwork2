using AutoMapper;
using MassTransit;
using MessagingModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Users.Data;
using Users.Dtos;
using Users.Extensions;
using Users.Models;

namespace Users.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly IUserRepo _repository;
		private readonly IMapper _mapper;
		private readonly IPublishEndpoint _publishEndPoint;

		public UsersController(
			IUserRepo repository,
			IMapper mapper,
			IPublishEndpoint publishEndPoint
		)
		{
			_repository = repository;
			_mapper = mapper;
			_publishEndPoint = publishEndPoint;
		}

		[HttpGet]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		public ActionResult<IEnumerable<UserReadDto>> GetUsers()
		{
			var users = _repository.GetUsers();

			return Ok(users);
		}

		[HttpGet("{id}", Name = "GetUser")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		public async Task<ActionResult<UserReadDto>> GetUserAsync(int id)
		{
			var userItem = _repository.GetUser(id);

			var token = await HttpContext.GetTokenAsync("access_token");

			if (userItem != null)
			{
				// return UserReadDto
				return Ok(_mapper.Map<UserReadDto>(userItem));
			}

			return NotFound();
		}

		[HttpPut("{id}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		public async Task<ActionResult> UpdateUser(int id, UserUpdateDto userUpdateDto)
		{
			if (!_repository.UserExists(id))
			{
				return BadRequest();
			}

			// check if user is not owner and not admin or moderator
			var userId = this.User.GetId();

			if (userId != id.ToString() && (!this.User.GetRoles().Contains("Admin") || !this.User.GetRoles().Contains("Moderator")))
			{
				return Unauthorized();
			}

			// map UserUpdateDto to User
			var userModel = _mapper.Map<User>(userUpdateDto);
			userModel.Id = id;

			// create user
			_repository.UpdateUser(userModel);
			// save to db
			_repository.SaveChanges();

			try
			{
				// Publish UserUpdated event
				await _publishEndPoint.Publish<UserUpdated>(new
				{
					Id = userModel.Id,
					Name = userModel.Name
				});
			}
			catch (Exception ex)
			{
				Console.WriteLine($"--> Could not send asynchronously: {ex.Message}");
			}

			return NoContent();
		}

		[HttpDelete("{id}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, Moderator, User")]
		public async Task<ActionResult> DeleteUser(int id)
		{
			var userId = this.User.GetId();

			// check if user is owner or admin or moderator
			if (userId == id.ToString() || this.User.GetRoles().Contains("Admin") || this.User.GetRoles().Contains("Moderator"))
			{

				var user = _repository.GetUser(id);

				if (user == null)
				{
					return NotFound("User was not found");
				}

				_repository.DeleteUser(user);

				// save to db
				if (_repository.SaveChanges())
				{
					try
					{
						// Publish UserDeleted event
						await _publishEndPoint.Publish<UserDeleted>(new
						{
							Id = user.Id,
						});
					}
					catch (Exception ex)
					{
						Console.WriteLine($"--> Could not send asynchronously: {ex.Message}");
					}

					return Ok();
				}

				return BadRequest("User cannot be removed");
			}

			return Unauthorized();
		}
	}
}
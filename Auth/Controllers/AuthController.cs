using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Auth.Dtos;
using AutoMapper;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MessagingModels;
using Auth.Models;

namespace Auth.Controllers
{
	[Route("api/[controller]")] // api/auth
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly UserManager<User> _userManager;
		private readonly RoleManager<Role> _roleManager;
		private readonly ILogger<AuthController> _logger;
		private readonly IConfiguration _configuration;
		private readonly IMapper _mapper;
		private readonly IPublishEndpoint _publishEndPoint;
		private readonly IWebHostEnvironment _hostingEnvironment;

		public AuthController(
				UserManager<User> userManager,
				RoleManager<Role> roleManager,
				ILogger<AuthController> logger,
				IConfiguration configuration,
				IMapper mapper,
				IPublishEndpoint publishEndPoint,
				IWebHostEnvironment hostingEnvironment)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_logger = logger;
			_configuration = configuration;
			_mapper = mapper;
			_publishEndPoint = publishEndPoint;
			_hostingEnvironment = hostingEnvironment;
		}

		/// <summary>
		/// Register
		/// </summary>
		/// <param name="userCreateDto"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("Register")]
		public async Task<IActionResult> Register([FromBody] UserCreateDto userCreateDto)
		{
			// create user
			var newUser = _mapper.Map<User>(userCreateDto);

			var isCreated = await _userManager.CreateAsync(newUser, userCreateDto.Password);

			if (isCreated.Succeeded)
			{
				// add the user to a role
				await _userManager.AddToRoleAsync(newUser, "User");

				// generate token
				var authResult = await GenerateJwtToken(newUser);

				// publish UserCreated event
				await _publishEndPoint.Publish<UserCreated>(new
				{
					Id = newUser.Id,
					Name = newUser.Name,
					Email = newUser.Email
				});

				return Ok(authResult);
			}

			return BadRequest(new AuthResult()
			{
				Errors = isCreated.Errors.Select(x => x.Description).ToList(),
				Success = false
			});
		}

		/// <summary>
		/// Login
		/// </summary>
		/// <param name="userLoginDto"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("Login")]
		public async Task<IActionResult> Login([FromBody] UserLoginDto userLoginDto)
		{
			var existingUser = await _userManager.FindByEmailAsync(userLoginDto.Email);

			if (existingUser == null)
			{
				return BadRequest(new AuthResult()
				{
					Errors = new List<string> {
						"Invalid login request"
					},
					Success = false
				});
			}

			var isCorrect = await _userManager.CheckPasswordAsync(existingUser, userLoginDto.Password);

			if (!isCorrect)
			{
				return BadRequest(new AuthResult()
				{
					Errors = new List<string> {
						"Invalid login request"
					},
					Success = false
				});
			}

			var authResult = await GenerateJwtToken(existingUser);

			return Ok(authResult);
		}

		/// <summary>
		/// Create new role
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("Roles")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
		public async Task<IActionResult> CreateRole([FromBody] RoleCreateDto roleCreateDto)
		{
			// check if role already exists
			var roleExists = await _roleManager.RoleExistsAsync(roleCreateDto.Name);

			if (!roleExists)
			{
				var roleResult = await _roleManager.CreateAsync(new Role(roleCreateDto.Name));

				// check if role was added successfully
				if (roleResult.Succeeded)
				{
					_logger.LogInformation($"Role {roleCreateDto.Name} has been added successfully");
					return Ok($"Role {roleCreateDto.Name} has been added successfully");
				}

				_logger.LogInformation($"Role {roleCreateDto.Name} has not been added successfully");
				return BadRequest($"Role {roleCreateDto.Name} has not been added successfully");
			}

			_logger.LogInformation($"Role {roleCreateDto.Name} already exists");
			return Conflict($"Role {roleCreateDto.Name} already exists");
		}

		/// <summary>
		/// Add user to role
		/// </summary>
		/// <param name="email"></param>
		/// <param name="roleName"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("AddUserToRole")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
		public async Task<IActionResult> AddUserToRole([FromBody] RoleUserDto roleUserDto)
		{
			// check if user exists
			var user = await _userManager.FindByEmailAsync(roleUserDto.Email);

			if (user == null)
			{
				_logger.LogInformation($"User with email {roleUserDto.Email} does not exist");
				return BadRequest($"User with email {roleUserDto.Email} does not exist");
			}

			// check if role exists
			var roleExists = await _roleManager.RoleExistsAsync(roleUserDto.RoleName);

			if (!roleExists)
			{
				_logger.LogInformation($"Role {roleUserDto.RoleName} does not exist");
				return BadRequest($"Role {roleUserDto.RoleName} does not exist");
			}

			// check if role is already assigned to the user
			var isInRole = await _userManager.IsInRoleAsync(user, roleUserDto.RoleName);

			if (isInRole)
			{
				_logger.LogInformation($"Role {roleUserDto.RoleName} is already assigned to user with email {roleUserDto.Email}");
				return Conflict($"Role {roleUserDto.RoleName} is already assigned to user with email {roleUserDto.Email}");
			}

			// check if user assigned to the role successfully
			var result = await _userManager.AddToRoleAsync(user, roleUserDto.RoleName);

			if (result.Succeeded)
			{
				return Ok($"Success, user with email {roleUserDto.Email} has been added to role {roleUserDto.RoleName}");
			}

			_logger.LogInformation($"User with email {roleUserDto.Email} was not added to role {roleUserDto.RoleName}");
			return BadRequest($"User with email {roleUserDto.Email} was not added to role {roleUserDto.RoleName}");
		}

		/// <summary>
		/// Remove user from role
		/// </summary>
		/// <param name="email"></param>
		/// <param name="roleName"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("RemoveUserFromRole")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
		public async Task<IActionResult> RemoveUserFromRole([FromBody] RoleUserDto roleUserDto)
		{
			// check if user exists
			var user = await _userManager.FindByEmailAsync(roleUserDto.Email);

			if (user == null)
			{
				_logger.LogInformation($"User with email {roleUserDto.Email} does not exist");
				return BadRequest($"User with email {roleUserDto.Email} does not exist");
			}

			// check is role exists
			var roleExists = await _roleManager.RoleExistsAsync(roleUserDto.RoleName);

			if (!roleExists)
			{
				_logger.LogInformation($"Role {roleUserDto.RoleName} does not exist");
				return BadRequest($"Role {roleUserDto.RoleName} does not exist");
			}

			// check if role is assigned to the user
			var isInRole = await _userManager.IsInRoleAsync(user, roleUserDto.RoleName);

			if (!isInRole)
			{
				_logger.LogInformation($"Role {roleUserDto.RoleName} is not assigned to user with email {roleUserDto.Email}");
				return BadRequest($"Role {roleUserDto.RoleName} is not assigned to user with email {roleUserDto.Email}");
			}

			// remove role
			var result = await _userManager.RemoveFromRoleAsync(user, roleUserDto.RoleName);

			if (result.Succeeded)
			{
				return Ok($"User with email {roleUserDto.Email} has been removed from role {roleUserDto.RoleName}");
			}

			return BadRequest($"Unable to remove User with email {roleUserDto.Email} from role {roleUserDto.RoleName}");
		}

		/// <summary>
		/// Generate JWT token
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		private async Task<AuthResult> GenerateJwtToken(User user)
		{
			var jwtTokenHandler = new JwtSecurityTokenHandler();

			var key = new byte[] { };
			// get security key based on environment
			if (_hostingEnvironment.IsProduction())
			{
				key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("JWT"));
			}
			else
			{
				key = Encoding.ASCII.GetBytes(_configuration.GetSection("JwtConfig")["Secret"]);
			}

			var claims = await GetAllValidClaims(user);

			// security token descriptor (contains claims)
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(claims),
				Expires = DateTime.UtcNow.AddHours(6),
				// sigining credentials (type of algorithm used to encrypt token)
				SigningCredentials = new SigningCredentials(
					new SymmetricSecurityKey(key),
					SecurityAlgorithms.HmacSha256Signature
				)
			};

			// security token
			var token = jwtTokenHandler.CreateToken(tokenDescriptor);
			var jwtToken = jwtTokenHandler.WriteToken(token);

			return new AuthResult()
			{
				Token = jwtToken,
				Success = true,
			};
		}

		/// <summary>
		/// Get all the valid claims for the user
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		private async Task<List<Claim>> GetAllValidClaims(User user)
		{
			// generic list of claims
			var claims = new List<Claim> {
				new Claim("Id", user.Id.ToString()),
				new Claim(JwtRegisteredClaimNames.Email, user.Email),
				new Claim(JwtRegisteredClaimNames.Sub, user.Email),
				// unique id used to generate a new token
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
			};

			// get claims that were assigned to user
			var userClaims = await _userManager.GetClaimsAsync(user);
			claims.AddRange(userClaims);

			// get user roles and add it to the claims
			var userRoles = await _userManager.GetRolesAsync(user);

			foreach (var userRole in userRoles)
			{
				var role = await _roleManager.FindByNameAsync(userRole);

				if (role != null)
				{
					claims.Add(new Claim(ClaimTypes.Role, userRole));

					// get claims assigned to each role
					var roleClaims = await _roleManager.GetClaimsAsync(role);

					foreach (var roleClaim in roleClaims)
					{
						claims.Add(roleClaim);
					}
				}
			}

			return claims;
		}
	}
}
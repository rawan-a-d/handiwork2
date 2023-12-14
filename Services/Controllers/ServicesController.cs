using AutoMapper;
using MassTransit;
using MessagingModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Data;
using Services.Dtos;
using Services.Extensions;
using Services.Models;

namespace Services.Controllers
{
	[Route("api/users/{userId}/[controller]")]
	[ApiController]
	public class ServicesController : ControllerBase
	{
		private readonly IServiceRepo _serviceRepository;
		private readonly IServiceCategoryRepo _serviceCategoryRepository;
		private readonly IPhotoRepo _photoRepository;
		private readonly IPhotoService _photoService;
		private readonly IMapper _mapper;
		private readonly IPublishEndpoint _publishEndPoint;

		public ServicesController(
			IServiceRepo serviceRepository,
			IServiceCategoryRepo serviceCategoryRepository,
			IPhotoRepo photoRepository,
			IPhotoService photoService,
			IMapper mapper,
			IPublishEndpoint publishEndPoint)
		{
			_serviceRepository = serviceRepository;
			_serviceCategoryRepository = serviceCategoryRepository;
			_photoRepository = photoRepository;
			_photoService = photoService;
			_mapper = mapper;
			_publishEndPoint = publishEndPoint;
		}

		/// <summary>
		/// Create new service
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="serviceCreateDto"></param>
		/// <returns></returns>
		[HttpPost]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		public async Task<ActionResult> CreateService(int userId, ServiceCreateDto serviceCreateDto)
		{
			// check if category exists
			var serviceCategory = _serviceCategoryRepository.GetServiceCategory(serviceCreateDto.ServiceCategoryId);

			if (serviceCategory == null)
			{
				return BadRequest("Service category does not exist");
			}

			// check if userId is the same as the current user id
			var userIdInToken = this.User.GetId();

			if (userIdInToken != userId.ToString())
			{
				return Unauthorized();
			}

			var serviceModel = _mapper.Map<Service>(serviceCreateDto);

			_serviceRepository.CreateService(userId, serviceModel);
			_serviceRepository.SaveChanges();

			// Publish ServiceCreated event
			await _publishEndPoint.Publish<ServiceCreated>(new
			{
				Id = serviceModel.Id,
				Info = serviceModel.Info,
				UserId = serviceModel.UserId,
				ServiceCategoryId = serviceModel.ServiceCategoryId
			});

			var serviceReadDto = _mapper.Map<ServiceReadDto>(serviceModel);

			return CreatedAtRoute(
				nameof(GetService),
				new
				{
					userId = userId,
					serviceId = serviceReadDto.Id
				},
				serviceReadDto
			);
		}

		/// <summary>
		/// Get services for a user
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		public ActionResult<IEnumerable<ServiceReadDto>> GetServices(int userId)
		{
			var services = _mapper.Map<IEnumerable<ServiceReadDto>>(_serviceRepository.GetServicesForUser(userId));

			return Ok(services);
		}

		/// <summary>
		/// Get service by id and user id
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="serviceId"></param>
		/// <returns></returns>
		[HttpGet("{serviceId}", Name = "GetService")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		public ActionResult<ServiceReadDto> GetService(int userId, int serviceId)
		{
			// get service
			var service = _serviceRepository.GetService(serviceId, userId);

			if (service == null)
			{
				return NotFound();
			}

			return Ok(_mapper.Map<ServiceReadDto>(service));
		}

		/// <summary>
		/// Update service
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="serviceId"></param>
		/// <param name="serviceUpdateDto"></param>
		/// <returns></returns>
		[HttpPut("{serviceId}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		public async Task<ActionResult<ServiceReadDto>> UpdateService(int userId, int serviceId, ServiceUpdateDto serviceUpdateDto)
		{
			// check if userId is the same as the current user id
			var userIdInToken = this.User.GetId();

			if (userIdInToken != userId.ToString())
			{
				return Unauthorized();
			}

			var service = _serviceRepository.GetService(serviceId, userId);
			if (service == null)
			{
				return NotFound();
			}

			// validate user is owner
			if (service.UserId != userId)
			{
				return Unauthorized();
			}

			// map ServiceUpdateDto to Service
			_mapper.Map(serviceUpdateDto, service);

			// update service
			_serviceRepository.UpdateService(service);
			// save to db
			_serviceRepository.SaveChanges();

			// Publish ServiceUpdated event
			await _publishEndPoint.Publish<ServiceUpdated>(new
			{
				Id = service.Id,
				Info = service.Info
			});

			return NoContent();
		}

		/// <summary>
		/// Delete service
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="serviceId"></param>
		/// <returns></returns>
		[HttpDelete("{serviceId}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		public async Task<ActionResult> DeleteService(int userId, int serviceId)
		{
			// check if userId is the same as the current user id
			var userIdInToken = this.User.GetId();

			if (userIdInToken != userId.ToString())
			{
				return Unauthorized();
			}

			// get service
			var service = _serviceRepository.GetService(serviceId, userId);

			// validate user is owner
			if (service.UserId != userId)
			{
				return Unauthorized();
			}

			_serviceRepository.DeleteService(service);

			if (_serviceRepository.SaveChanges())
			{
				// Publish ServiceDeleted event
				await _publishEndPoint.Publish<ServiceDeleted>(new
				{
					Id = service.Id
				});

				return Ok();
			}

			return BadRequest("Service cannot be removed");
		}

		[HttpPost("{serviceId}/photos")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		public async Task<ActionResult<PhotoDto>> AddPhoto(int userId, int serviceId, IFormFile file)
		{
			// check if userId is the same as the current user id
			var userIdInToken = this.User.GetId();

			if (userIdInToken != userId.ToString())
			{
				return Unauthorized();
			}

			// get service object with the photos
			var service = _serviceRepository.GetService(serviceId, userId);

			if (service == null)
			{
				return NotFound("Service does not exist");
			}

			// validate user is owner
			if (service.UserId != userId)
			{
				return Unauthorized();
			}

			// add new photo to Cloudinary
			var result = await _photoService.AddPhotoAsync(file);

			// if there was an error
			if (result.Error != null)
			{
				return BadRequest(result.Error.Message);
			}

			// Create new photo object using the result
			var photo = new Photo
			{
				Url = result.SecureUrl.AbsoluteUri,
				PublicId = result.PublicId
			};

			// add photo to photos array
			//_photoRepository.CreatePhoto(photo);
			service.Photos.Add(photo);

			// save changes to db
			if (_photoRepository.SaveChanges())
			{
				return CreatedAtRoute("GetService", new { userId = userId, serviceId = serviceId }, _mapper.Map<PhotoDto>(photo));
			}

			return BadRequest("Problem adding photos");
		}

		/// <summary>
		/// Delete a photo
		/// </summary>
		/// <param name="photoId">photo id</param>
		/// <returns></returns>
		[HttpDelete("{serviceId}/photos/{photoId}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		public async Task<ActionResult> DeletePhoto(int userId, int serviceId, int photoId)
		{
			// check if userId is the same as the current user id
			var userIdInToken = this.User.GetId();

			if (userIdInToken != userId.ToString())
			{
				return Unauthorized();
			}

			// get service object with the photos
			var service = _serviceRepository.GetService(serviceId, userId);

			if (service == null)
			{
				return NotFound("Service does not exist");
			}

			// validate user is owner
			if (service.UserId != userId)
			{
				return Unauthorized();
			}

			// find photo
			var photo = service.Photos.FirstOrDefault(x => x.Id == photoId);

			if (photo == null)
			{
				return NotFound();
			}

			// remove photo
			// 1. from Cloudinary
			if (photo.PublicId != null)
			{
				// delete photo from Cloudinary
				var result = await _photoService.DeletePhotoAsync(photo.PublicId);

				if (result.Error != null)
				{
					return BadRequest(result.Error.Message);
				}
			}
			// 2. from DB
			_photoRepository.DeletePhoto(photo);

			// save to db
			if (_photoRepository.SaveChanges())
			{
				return Ok();
			}

			return BadRequest("Failed to delete photo");
		}

		[HttpGet("search")]
		public ActionResult<IEnumerable<User>> SearchBy(string keyword)
		{
			var users = _mapper.Map<IEnumerable<UserReadDto>>(_serviceRepository.SearchService(keyword));

			return Ok(users);
		}
	}
}
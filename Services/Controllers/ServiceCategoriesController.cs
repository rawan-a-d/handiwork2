using AutoMapper;
using MassTransit;
using MessagingModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Data;
using Services.Dtos;
using Services.Models;

namespace Services.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ServiceCategoriesController : ControllerBase
	{
		private readonly IServiceCategoryRepo _repository;
		private readonly IMapper _mapper;
		private readonly IPublishEndpoint _publishEndPoint;

		public ServiceCategoriesController(
			IServiceCategoryRepo repository,
			IMapper mapper,
			IPublishEndpoint publishEndPoint)
		{
			_mapper = mapper;
			_repository = repository;
			_publishEndPoint = publishEndPoint;
		}

		/// <summary>
		/// Create new service category
		/// </summary>
		/// <param name="serviceCategoryCreateDto"></param>
		/// <returns></returns>
		[HttpPost]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, Moderator")]
		public async Task<ActionResult<ServiceCategoryReadDto>> CreateServiceCategory(ServiceCategoryCreateDto serviceCategoryCreateDto)
		{
			if (_repository.DoesServiceCategoryExist(serviceCategoryCreateDto.Name))
			{
				return Conflict("Service category already exists");
			}

			var serviceCategoryModel = _mapper.Map<ServiceCategory>(serviceCategoryCreateDto);
			_repository.CreateServiceCategory(serviceCategoryModel);
			_repository.SaveChanges();

			// Publish ServiceCategoryCreated event
			await _publishEndPoint.Publish<ServiceCategoryCreated>(new
			{
				Id = serviceCategoryModel.Id,
				Name = serviceCategoryModel.Name,
			});

			var serviceCategoryReadDto = _mapper.Map<ServiceCategoryReadDto>(serviceCategoryModel);

			return CreatedAtRoute(
				nameof(GetServiceCategory),
				new { Id = serviceCategoryReadDto.Id },
				serviceCategoryReadDto
			);
		}

		/// <summary>
		/// Get service categories
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public ActionResult<IEnumerable<ServiceCategoryReadDto>> GetServiceCategories()
		{
			var categories = _repository.GetServiceCategories();

			return Ok(_mapper.Map<IEnumerable<ServiceCategoryReadDto>>(categories));
		}

		/// <summary>
		/// Get service category by id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet("{id}", Name = "GetServiceCategory")]
		public ActionResult<ServiceCategoryReadDto> GetServiceCategory(int id)
		{
			var categoryModel = _repository.GetServiceCategory(id);

			if (categoryModel == null)
			{
				return NotFound();
			}

			return Ok(_mapper.Map<ServiceCategoryReadDto>(categoryModel));
		}

		/// <summary>
		/// Update service category
		/// </summary>
		/// <param name="id"></param>
		/// <param name="serviceCategoryUpdateDto"></param>
		/// <returns></returns>
		[HttpPut("{id}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, Moderator")]
		public async Task<ActionResult> UpdateServiceCategory(int id, ServiceCategoryUpdateDto serviceCategoryUpdateDto)
		{
			var serviceCategoryModel = _repository.GetServiceCategory(id);

			if (serviceCategoryModel == null)
			{
				return NotFound();
			}

			// map ServiceCategoryUpdateDto to ServiceCategory
			_mapper.Map(serviceCategoryUpdateDto, serviceCategoryModel);

			// update service
			_repository.UpdateServiceCategory(serviceCategoryModel);
			// save to db
			_repository.SaveChanges();

			// Publish ServiceCategoryUpdated event
			await _publishEndPoint.Publish<ServiceCategoryUpdated>(new
			{
				Id = serviceCategoryModel.Id,
				Name = serviceCategoryModel.Name
			});

			return NoContent();
		}

		/// <summary>
		/// Delete service category by id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpDelete("{id}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, Moderator")]
		public async Task<ActionResult> DeleteServiceCategory(int id)
		{
			var serviceCategoryModel = _repository.GetServiceCategory(id);

			if (serviceCategoryModel == null)
			{
				return NotFound();
			}

			_repository.DeleteServiceCategory(serviceCategoryModel);

			if (_repository.SaveChanges())
			{
				// Publish ServiceCategoryDeleted event
				await _publishEndPoint.Publish<ServiceCategoryDeleted>(new
				{
					Id = serviceCategoryModel.Id
				});

				return Ok();
			}

			return BadRequest("Service category cannot be removed");
		}
	}
}
using AutoMapper;
using MessagingModels;
using Services.Dtos;
using Services.Models;

namespace Services.Profiles
{
	public class ServicesProfile : Profile
	{
		public ServicesProfile()
		{
			// Source -> Target
			CreateMap<Service, ServiceReadDto>();
			CreateMap<ServiceCreateDto, Service>();
			CreateMap<ServiceUpdateDto, Service>();

			CreateMap<ServiceCategory, ServiceCategoryReadDto>();
			CreateMap<ServiceCategoryCreateDto, ServiceCategory>();
			CreateMap<ServiceCategoryUpdateDto, ServiceCategory>();

			CreateMap<Photo, PhotoDto>();

			CreateMap<User, UserReadDto>();

			CreateMap<UserCreated, User>()
				.ForMember(dest => dest.ExternalId, opt =>
					opt.MapFrom(src => src.Id)
				)
				.ForMember(dest => dest.Id, opt =>
					opt.Ignore()
				);
			//.ForMember(dest => dest.Id, );
			CreateMap<UserUpdated, User>()
				.ForMember(dest => dest.ExternalId, opt =>
					opt.MapFrom(src => src.Id)
				)
				.ForMember(dest => dest.Id, opt =>
					opt.Ignore()
				);
			CreateMap<UserDeleted, User>()
				.ForMember(dest => dest.ExternalId, opt =>
					opt.MapFrom(src => src.Id)
				)
				.ForMember(dest => dest.Id, opt =>
					opt.Ignore()
				);
		}
	}
}
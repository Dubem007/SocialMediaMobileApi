using AutoMapper;
using Domain.Entities;
using Domain.Entities.Identity;
using Shared.DataTransferObjects;

namespace Application.Mapper
{
    public class UserMemberMapper : Profile
    {
        public UserMemberMapper()
        {
            CreateMap<UserMemberCreationInputDto, UserMember>();
            CreateMap<DeviceTokenCreateDto, DeviceToken>();
            CreateMap<DeviceToken, DeviceTokenDto >();
            CreateMap<UserMember, UserMemberResponseDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom((src, dest) => dest.Id = src.User.Id))
                .ForMember(dest => dest.UserMemberId, opt => opt.MapFrom((src, dest) => dest.UserMemberId = src.Id))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom((src, dest) => dest.FirstName = src.User.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom((src, dest) => dest.LastName = src.User.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom((src, dest) => dest.Email = src.User.Email))
                .ForMember(dest => dest.ProfileImage, opt => opt.MapFrom((src, dest) => dest.ProfileImage = src.User.ImageUrl));
            CreateMap<UserMemberUpdateInputDto, UserMember>()
                .AfterMap((src, dest) => dest.User.FirstName = src.FirstName)
                .AfterMap((src, dest) => dest.User.LastName = src.LastName);
            CreateMap<UserMemberCreationInputDto, UserMemberResponseDto>()
                .ForMember(dest => dest.ProfileImage, opt => opt.Ignore());
            CreateMap<User, UserLoginResponse>().ReverseMap();
        }
    }
}

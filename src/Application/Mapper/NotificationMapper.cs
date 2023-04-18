using AutoMapper;
using Domain.Entities;
using Shared.DataTransferObjects;

namespace Application.Mapper
{
    public class NotificationMapper:Profile
    {
        public NotificationMapper()
        {
            CreateMap<Notification, NotificationDTO>()
                   .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Sender.FirstName))
                   .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Sender.LastName))
                   .ForMember(dest => dest.ProfileImage, opt => opt.MapFrom(src => src.Sender.ImageUrl));
            CreateMap<NotificationCreateDTO, Notification>().ReverseMap();
        }
    }
}

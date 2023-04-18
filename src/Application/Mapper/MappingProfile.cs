using AutoMapper;
using Domain.Entities;
using Domain.Entities.Identity;
using Shared.DataTransferObjects;

namespace Application.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<InvitesDto, UserMember>().ReverseMap();
            CreateMap<MemberDto, User>();
            CreateMap<Subscription, SubscriptionResponseDto>().ReverseMap();
            CreateMap<PremiumPlan, PremiumPlansResponseDto>().ReverseMap();
        }
    }
}

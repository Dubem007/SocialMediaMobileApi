using AutoMapper;
using Domain.Entities;
using Shared.DataTransferObjects;

namespace Application.Mapper
{
    public class InitialMemberMapper : Profile
    {
        public InitialMemberMapper()
        {
            CreateMap<InitialMember, InitialMemberResponseDto>();
            CreateMap<InitialMember, GetInitialMemberTokenResponseDto>();
            CreateMap<UserMember, SearchUserMemberDto>().ReverseMap();
            CreateMap<MemberGroupDto, GroupUserMembersDto>();
            CreateMap<UploadInitialMemeberDto, InitialMember>();
            CreateMap<UploadProfessionalListDto, Professions>();
            CreateMap<Professions, ProfessionalListResponseDto > ();
            CreateMap<UploadSubscriptionDto, Subscription>();
            CreateMap<Subscription, SubscriptionResponseDto>();
            CreateMap<UploadPremiumPlansDto, PremiumPlan>();
            CreateMap<PremiumPlan, PremiumPlansResponseDto>();
            CreateMap<UploadMemberGroupDto, MemberGroup>();
            CreateMap<MemberGroup, MemberGroupResponseDto>();
        }
    }
}

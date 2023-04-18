using Application.Helpers;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Shared.DataTransferObjects;
using Shared.ResourceParameters;

namespace Application.Contracts
{
    public interface IUserMemberService : IAutoDependencyService
    {
        Task<SuccessResponse<UserMemberResponseDto>> CreateUserMember(UserMemberCreationInputDto input);
        Task<SuccessResponse<UserMemberResponseDto>> GetUserMemberById(Guid input);
        Task<PagedResponse<IEnumerable<UserMemberResponseDto>>> GetUserMembers(SearchParameters parameters, string actionName, IUrlHelper urlHelper);
        //Task<PagedResponse<IEnumerable<UserMemberResponseDto>>> GetUserMembers(UserMemberParameters parameters, string actionName, IUrlHelper urlHelper);
        Task<SuccessResponse<UserMemberResponseDto>> UpdateUserMember(UserMemberUpdateInputDto input);
        Task<SuccessResponse<IEnumerable<GroupUserMembersDto>>> MembersByGroupCountAsync();
        Task<SuccessResponse<GroupUserMembersDto>> MembersGroupByIdAsync(Guid Id);
        Task<SuccessResponse<IEnumerable<ListOfProfessionsDto>>> GetAllProfessionalFields();
        Task<PagedResponse<IEnumerable<UserMemberResponseDto>>> GetUserMembersForUser(SearchUserMembersByUserParameters parameters, string value, string actionName, IUrlHelper urlHelper);
        Task<LocationDto> Location(MembersByLocationParameters parameters, string actionName, IUrlHelper urlHelper);
        Task<Predictions> LocationPredictions(MembersByLocationParameters parameters, string actionName, IUrlHelper urlHelper);
        Task<LocationDto> LongitudeAndLatitlude(LocationByPlaceIdParameters parameters, string actionName, IUrlHelper urlHelper);
        Task<PagedResponse<IEnumerable<MembersByLocationDto>>> ConnectionsNearby(ResourceParameters parameters, string actionName, IUrlHelper urlHelper);
        Task<PagedResponse<IEnumerable<BreakdownDto>>> GetAllByTheLocationAsync(string actionName, IUrlHelper urlHelper);
        Task<SuccessResponse<UserMemberResponseDto>> UpdateUserPhoto(UserMemberPhotoUpdateInputDto input);
        Task<PagedResponse<IEnumerable<UserMemberResponseDto>>> GetUserMembersSuggestionsForUser(string actionName, IUrlHelper urlHelper);
        Task<SuccessResponse<DeviceTokenDto>> CreateDeviceToken(DeviceTokenCreateDto model);
        Task<PagedResponse<IEnumerable<UserMemberResponseDto>>> GetUsersPerLocationAsync(SearchUserMembersBylocationParameters parmmeters, string search, string actionName, IUrlHelper urlHelper);
    }
}

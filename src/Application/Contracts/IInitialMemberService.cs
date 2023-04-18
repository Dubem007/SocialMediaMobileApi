using Application.Helpers;
using Shared.DataTransferObjects;

namespace Application.Contracts
{
    public interface IInitialMemberService : IAutoDependencyService
    {
        Task<SuccessResponse<InitialMemberResponseDto>> GetInitialMemberByEmail(InitialMemberInputDto input);
        Task<SuccessResponse<UploadInitialMemberResponseDto>> UploadBulkInitialMembers(UploadInitialMemberInputDto input);
        Task<SuccessResponse<UploadProfessionalListResponseDto>> UploadBulkProfessionalList(UploadProfessionalListInputDto input);
        Task<SuccessResponse<UploadSubscriptionResponseDto>> UploadSubscriptions(UploadSubscriptionInputDto input);
        Task<SuccessResponse<UploadPremiumPlansResponseDto>> UploadPremiumPlans(UploadPremiumPlansInputDto input);
        Task<SuccessResponse<UploadMemberGroupResponseDto>> UploadMemberGroup(UploadMemberGroupInputDto input);
    }
}

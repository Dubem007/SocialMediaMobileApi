using Application.Helpers;
using Microsoft.AspNetCore.Mvc;
using Shared.DataTransferObjects;
using Shared.ResourceParameters;

namespace Application.Contracts
{
    public interface ISubscriptionsService : IAutoDependencyService
    {
        Task<SuccessResponse<IEnumerable<SubscriptionResponseDto>>> GetAllSubscriptionsAsync();
        Task<SuccessResponse<IEnumerable<PremiumPlansResponseDto>>> GetPremiumPlansAsync();
    }
}

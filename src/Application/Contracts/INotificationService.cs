using Application.Helpers;
using Microsoft.AspNetCore.Mvc;
using Shared.DataTransferObjects;

namespace Application.Contracts
{
    public interface INotificationService
    {
        Task<SuccessResponse<NotificationDTO>> CreateNotification(NotificationCreateDTO model);
        Task<int> GetUnReadNotificationStats(Guid memberId);
        Task<SuccessResponse<bool>> ToggleNotificationStatus(IEnumerable<Guid> ids);
        Task<PagedResponse<IEnumerable<NotificationDTO>>> GetMembersNotifications(Guid memberId, ResourceParameter parameter, string name, IUrlHelper urlHelper);
        
        
    }
}

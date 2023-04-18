namespace Infrastructure.Contracts;

public interface IRepositoryManager
{
    IUserRepository User { get; }
    IInvitationRepository Invitation { get; }
    IUserMemberRepository UserMember { get; }
    IMessageRepository ChatMessage { get; }
    INotificationRepository Notification { get; }
    ITokenRepository Token { get; }
    IUserActivityRepository UserActivity { get; }
    IUserConnectionRepository UserConnection { get; }
    IInitialMemberRepository InitialMember { get; }
    IMemberGroupRepository MemberGroup { get; }
    IChatHistoryRepository ChatHistory { get; }
    ISubscriptionsRepository Subscriptions { get; }
    IPremiumPlansRepository PremiumPlans { get; }
    IProfessionalFieldRepository ProfessionalField { get; }
    IDeviceTokenRepository DeviceToken { get; }

    Task<int> SaveChangesAsync();
}
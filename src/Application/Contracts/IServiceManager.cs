namespace Application.Contracts
{
    public interface IServiceManager
    {
        IAuthenticationService AuthenticationService { get; }
        IInitialMemberService InitialMemberService { get; }
        IInvitationService InvitationService { get; }
        INotificationService NotificationService { get; }
        IUserMemberService UserMember { get; }
        IUserConnectionService UserConnectionService { get; }

     
        IUserMemberService UserMemberService { get; }
        IChatMessageService ChatMessageService { get; }
        ISubscriptionsService Subscriptions { get; }

    }
}

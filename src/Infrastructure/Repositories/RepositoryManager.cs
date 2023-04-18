using Infrastructure.Contracts;
using Infrastructure.Data.DbContext;

namespace Infrastructure.Repositories
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly AppDbContext _appDbContext;
        private readonly Lazy<IInvitationRepository> _invitationRepository;
        private readonly Lazy<IUserMemberRepository> _memberRepository;
        private readonly Lazy<IMessageRepository> _messageRepository;
        private readonly Lazy<INotificationRepository> _notificationRepository;
        private readonly Lazy<ITokenRepository> _tokenRepository;
        private readonly Lazy<IUserActivityRepository> _userActivityRepository;
        private readonly Lazy<IUserConnectionRepository> _userConnectionRepository;
        private readonly Lazy<IUserRepository> _userRepository;
        private readonly Lazy<IInitialMemberRepository> _initialMemberRepository;
        private readonly Lazy<IMemberGroupRepository> _memberGroupRepository;
        private readonly Lazy<IChatHistoryRepository> _chatHistoryRepository;
        private readonly Lazy<ISubscriptionsRepository> _subscriptionsRepository;
        private readonly Lazy<IPremiumPlansRepository> _premiumPlansRepository;
       private readonly Lazy<IProfessionalFieldRepository> _professionalFieldRepository;
       private readonly Lazy<IDeviceTokenRepository> _deviceTokenRepository;
        public RepositoryManager( AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
            _invitationRepository = new Lazy<IInvitationRepository>(() => new InvitationRepository(appDbContext));
            _memberRepository = new Lazy<IUserMemberRepository>(() => new UserMemberRepository(appDbContext));
            _messageRepository = new Lazy<IMessageRepository>(() => new MessageRepository(appDbContext));
            _notificationRepository = new Lazy<INotificationRepository>(() => new NotificationRepository(appDbContext));
            _tokenRepository = new Lazy<ITokenRepository>(() => new TokenRepository(appDbContext));
            _userActivityRepository = new Lazy<IUserActivityRepository>(() => new UserActivityRepository(appDbContext));
            _userConnectionRepository = new Lazy<IUserConnectionRepository>(() => new UserConnectionRepository(appDbContext));
            _userRepository = new Lazy<IUserRepository>(() => new UserRepository(appDbContext));
            _initialMemberRepository = new Lazy<IInitialMemberRepository>(() => new InitialMemberRepository(appDbContext));
            _memberGroupRepository = new Lazy<IMemberGroupRepository>(() => new MemberGroupRepository(appDbContext));
            _chatHistoryRepository = new Lazy<IChatHistoryRepository>(() => new ChatHistoryRepository(appDbContext));
            _subscriptionsRepository = new Lazy<ISubscriptionsRepository>(() => new SubscriptionsRepository(appDbContext));
            _premiumPlansRepository = new Lazy<IPremiumPlansRepository>(() => new PremiumPlansRepository(appDbContext));
            _professionalFieldRepository = new Lazy<IProfessionalFieldRepository>(() => new ProfessionalFieldRepository(appDbContext));
            _deviceTokenRepository = new Lazy<IDeviceTokenRepository>(() => new DeviceTokenRepository(appDbContext));
        }

        public IUserRepository User => _userRepository.Value;

        public IInvitationRepository Invitation => _invitationRepository.Value;

        public IUserMemberRepository UserMember => _memberRepository.Value;

        public IMessageRepository ChatMessage => _messageRepository.Value;

        public INotificationRepository Notification => _notificationRepository.Value;

        public ITokenRepository Token => _tokenRepository.Value;

        public IUserActivityRepository UserActivity => _userActivityRepository.Value;

        public IUserConnectionRepository UserConnection => _userConnectionRepository.Value;

        public IInitialMemberRepository InitialMember => _initialMemberRepository.Value;
        public IMemberGroupRepository MemberGroup => _memberGroupRepository.Value;
        public IChatHistoryRepository ChatHistory => _chatHistoryRepository.Value;

        public ISubscriptionsRepository Subscriptions => _subscriptionsRepository.Value;
        public IPremiumPlansRepository PremiumPlans => _premiumPlansRepository.Value;
        public IProfessionalFieldRepository ProfessionalField => _professionalFieldRepository.Value;
        public IDeviceTokenRepository DeviceToken => _deviceTokenRepository.Value;

        public async Task<int> SaveChangesAsync() => await _appDbContext.SaveChangesAsync();
    }
}

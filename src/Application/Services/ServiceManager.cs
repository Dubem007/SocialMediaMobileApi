using Application.Contracts;
using Application.Helpers;
using AutoMapper;
using Domain.Entities.Identity;
using Infrastructure.Contracts;
using Infrastructure.HttpHelper;
using Infrastructure.Logger;
using Infrastructure.Utils.AWS;
using Infrastructure.Utils.Email;
using Infrastructure.Utils.EmailClient;
using Infrastructure.Utils.Notification;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using Role = Domain.Entities.Identity.Role;

namespace Application.Services
{
    public class ServiceManager : IServiceManager
    {
        private readonly Lazy<IAuthenticationService> _authenticationService;
        private readonly Lazy<InitialMemberService> _initialMemberService;
        private readonly Lazy<IUserMemberService> _userMemberService;
        private readonly Lazy<IInvitationService> _invitationService;
        private readonly Lazy<INotificationService> _notificationService;
        private readonly Lazy<IChatMessageService> _chatMessageService;
        private readonly Lazy<ISubscriptionsService> _subscriptionsService;
        private readonly Lazy<ICacheServices> _cacheServices;
        private readonly Lazy<IUserConnectionService> _connectionService;


        public ServiceManager(IRepositoryManager repository,
            IMapper mapper,
            ILoggerManager loggerManager,
            IAwsS3Client awsS3Client,
            IConfiguration configuration,
            IEmailManager emailManager,
            UserManager<User> userManager,
            INotificationService notification,
            IWebHelper webHelper,
            RoleManager<Role> roleManager,
            INotificationManager notify,
            IHttpClientHelper clientHelper, ICacheServices cache,  IConnectionMultiplexer redis, IEmailClient emailClient)
        {
            _authenticationService = new Lazy<IAuthenticationService>(() => new AuthenticationService(mapper, configuration, userManager, roleManager, emailManager, emailClient, repository, awsS3Client, webHelper));
            _initialMemberService = new Lazy<InitialMemberService>(() => new InitialMemberService(mapper, repository));
            _invitationService = new Lazy<IInvitationService>(() => new InvitationService(repository, notification, webHelper));
            _userMemberService = new Lazy<IUserMemberService>(() => new UserMemberService(repository, awsS3Client, userManager, mapper, webHelper, clientHelper, cache));
            _connectionService = new Lazy<IUserConnectionService>(() => new UserConnectionService(repository, mapper, webHelper));
            _notificationService = new Lazy<INotificationService>(() => new NotificationService(mapper, configuration, repository,notify));
            _chatMessageService = new Lazy<IChatMessageService>(() => new ChatMessageService(repository, mapper, webHelper, awsS3Client));
            _subscriptionsService = new Lazy<ISubscriptionsService>(() => new SubscriptionsService(repository, mapper));
            _cacheServices = new Lazy<ICacheServices>(() => new CacheServices(repository, mapper, redis));
        }

        public IAuthenticationService AuthenticationService => _authenticationService.Value;
        public IInitialMemberService InitialMemberService => _initialMemberService.Value;
        public IInvitationService InvitationService => _invitationService.Value;
        public INotificationService NotificationService => _notificationService.Value;
        public IUserMemberService UserMember => _userMemberService.Value;
       

        public IUserConnectionService UserConnectionService => _connectionService.Value;
        public IUserMemberService UserMemberService => _userMemberService.Value;
        public IChatMessageService ChatMessageService => _chatMessageService.Value;
        public ISubscriptionsService Subscriptions => _subscriptionsService.Value;

    }
}

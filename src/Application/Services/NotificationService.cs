using Application.Contracts;
using Application.Helpers;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CorePush.Apple;
using CorePush.Google;
using Domain.ConfigurationModels;
using Domain.Entities;
using Infrastructure.Contracts;
using Infrastructure.Utils.Notification;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Shared.DataTransferObjects;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using static AutoMapper.Internal.ExpressionFactory;
using static Shared.DataTransferObjects.GoogleNotification;

namespace Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IRepositoryManager _repository;
        private readonly INotificationManager _notificationManager;


      
        public NotificationService(IMapper mapper, IConfiguration configuration, IRepositoryManager repository, INotificationManager notificationManager)
        {
            _mapper = mapper;
            _configuration = configuration;
            _repository = repository;
            _notificationManager = notificationManager;
        }

        public async Task<SuccessResponse<NotificationDTO>> CreateNotification(NotificationCreateDTO model)
        {
            var notification = _mapper.Map<Notification>(model);
            notification.CreatedById = model.SenderId;
            await _repository.Notification.CreateAsync(notification);
            await _repository.SaveChangesAsync();
            var response = _mapper.Map<NotificationDTO>(notification);
            response.Id = notification.Id;
            DataPayload dataPayload = new()
            {
                Title = model.Title,
                Body = model.Body
            };

            GoogleNotification gNotification = new()
            {
                Data = dataPayload,
                Notification = dataPayload
            };
            string deviceToken = await GetDeviceToken(notification.RecieverId);
             _notificationManager.SendNotification(deviceToken, gNotification);

            return new SuccessResponse<NotificationDTO>
            {
                Data = response,
                Message = "Notification sent successfully",
                Success = true
            };
        }
        public async Task<int> GetUnReadNotificationStats(Guid userId)
        {
           
             int count = await _repository.Notification.CountAsync(x => x.RecieverId == userId);

            return count;
        }

        public async Task<SuccessResponse<bool>> ToggleNotificationStatus(IEnumerable<Guid> ids)
        {
            if (!ids.Any())
                throw new RestException(System.Net.HttpStatusCode.BadRequest, "Notification cannot be null");

            foreach (var notification in ids)
            {
                var notificationToToggle = await _repository.Notification.FirstOrDefaultAsync((n => n.Id == notification && n.IsRead == false), true);
                if (notificationToToggle is not null)
                {
                    notificationToToggle.IsRead = true;
                    _repository.Notification.Update(notificationToToggle);
                    await _repository.SaveChangesAsync();
                }

            }

            return new SuccessResponse<bool>
            {
                Message = "Data Updated Successfully",
                Data = true
            };
        }
        public async Task<PagedResponse<IEnumerable<NotificationDTO>>> GetMembersNotifications(Guid userId, ResourceParameter parameter, string name, IUrlHelper urlHelper)
        {
            var isMemberExists = await _repository.User.ExistsAsync(x=>x.Id == userId);
            if (!isMemberExists)
                throw new RestException(HttpStatusCode.NotFound, $"Member with {userId} does not exist");

            var query = _repository.Notification.Get(x => x.RecieverId == userId);
            //query = query.Include(x => x.Sender).
            //    ThenInclude(x => x.User);
            if (!string.IsNullOrEmpty(parameter.FilterBy))
            {
                bool isRead = parameter.FilterBy.ToUpper() == "ISREAD" ? true : false;
                query = query.Where(x => x.IsRead == isRead);
            }
            query = query.OrderByDescending(x => x.CreatedAt);
            var projectedQuery = query.ProjectTo<NotificationDTO>(_mapper.ConfigurationProvider);

            var notifications = await PagedList<NotificationDTO>.Create(projectedQuery, parameter.PageNumber, parameter.PageSize);
            var notificationParameters = PageUtility<NotificationDTO>.GenerateResourceParameters(parameter, notifications);
            var page = PageUtility<NotificationDTO>.CreateResourcePageUrl(notificationParameters, name, notifications, urlHelper);

            return new PagedResponse<IEnumerable<NotificationDTO>>
            {
                Success = true,
                Message = "Notifications retrieved successfully",
                Data = notifications,
                Meta = new Meta
                {
                    Pagination = page
                }

            };
        }
        
        private async Task<string> GetDeviceToken(Guid userId)
        {
   
            var deviceToken = await _repository.DeviceToken.FirstOrDefaultAsync(x=>x.UserId == userId,false);
            
            return deviceToken?.DeviceId ?? "";

        }
        
      
      
       

        private async Task<Guid> GetMemberId(Guid id)
        {
           var userMember = await _repository.UserMember.FirstOrDefaultAsync(x => x.UserId == id, false);
            if (userMember == null)
            {
                throw new RestException(HttpStatusCode.NotFound, $"This User does not exist");
            }
            return userMember.Id;
        }
        
    }
}

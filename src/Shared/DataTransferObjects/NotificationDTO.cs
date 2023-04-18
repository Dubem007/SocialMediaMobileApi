

using Newtonsoft.Json;

namespace Shared.DataTransferObjects
{
    public record NotificationDTO
    {
        public Guid Id { get; set; }
        public bool IsRead { get; set; }
        public Guid RecieverId { get; set; }
        public Guid SenderId { get; set; }
        public string Title { get; set; }
        public string DeviceId { get; set; }
        public string Body { get; set; }
        public string Type { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfileImage { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
    public record NotificationCreateDTO
    {
        public Guid RecieverId { get; set; }
        public Guid SenderId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Type { get; set; }
    }
    public record GoogleNotification
    {
        public record DataPayload
        {
            public string Title { get; set; }
            public string Body { get; set; }
        }
        public string Priority { get; set; } = "high";
        public DataPayload Data { get; set; }
        public DataPayload Notification { get; set; }
    }

}

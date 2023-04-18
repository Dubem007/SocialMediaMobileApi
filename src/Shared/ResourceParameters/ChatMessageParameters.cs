namespace Shared.ResourceParameters
{
    public record ChatMessageParameters : ResourceParameters
    {
        public Guid RecipientId { get; set; }
        public string Search { get; set; }
    }
}

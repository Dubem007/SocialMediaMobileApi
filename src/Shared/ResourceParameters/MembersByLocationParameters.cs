namespace Shared.ResourceParameters
{
    public record MembersByLocationParameters : ResourceParameters
    {
        public string Location { get; set; }
    }

    public record LocationByPlaceIdParameters : ResourceParameters
    {
        public string PlaceId { get; set; }
    }
}

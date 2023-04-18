namespace Shared.ResourceParameters
{
    public record ConnectionsByCityParameter : ResourceParameters
    {
        public string City { get; set; }
        public string Search { get; set; }
    }
}

namespace Shared.ResourceParameters
{
    public record ResourceParameters
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string Sort { get; set; }
    }
}

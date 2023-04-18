namespace Shared.ResourceParameters
{
    public record UserMemberParameters : ResourceParameters
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string RecognitionYear { get; set; }
        public string ProfessionalField { get; set; }
        public string Location { get; set; }
        public string Email { get; set; }
    }

    public record SearchParameters : ResourceParameters
    {
        public string Search { get; set; }
    }

    public record UserMembersByUsersParameters : ResourceParameters
    {
        public Guid UserId { get; set; }
        public string RecognitionYear { get; set; }
        public string ProfessionalField { get; set; }
        public string Location { get; set; }
    }

    public record SearchUserMembersByUserParameters : ResourceParameters
    {
        public Guid UserId { get; set; }
        public string Search { get; set; }

    }
    public record SearchUserMembersBylocationParameters : ResourceParameters
    {
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string Search { get; set; }

    }
}

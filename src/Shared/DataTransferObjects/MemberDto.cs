namespace Shared.DataTransferObjects
{
    public record MemberDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ImageUrl { get; set; }

    }

    public record MembersByLocationDto : MemberDto
    {
        public Guid MemberId { get; set; }
        public Guid UserId { get; set; }
        public string Location { get; set; }
        public string Country { get; set; }
        public string ProfessionalField { get; set; }
    }
}

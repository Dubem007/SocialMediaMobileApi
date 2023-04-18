namespace Shared.DataTransferObjects
{
    public record ConnectionsDto
    {
        public Guid MemberId { get; set; }
        public string Country { get; set; }
        public string ProfessionalField { get; set; }
        public string Location { get; set; }
        public string ImageUrl { get; set; }
        public Guid UserId { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string FirstName { get; set; }
        public string Bio { get; set; }
        public string LastName { get; set; }
        public string PrefferedName { get; set; }
        public string Email { get; set; }

    }

    public record DisconnectDto
    {
        public Guid MemberId { get; set; }
        public bool Decision { get; set; }
    }
}

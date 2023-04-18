using CsvHelper.Configuration.Attributes;
using Microsoft.AspNetCore.Http;

namespace Shared.DataTransferObjects
{
    public record InitialMemberInputDto
    {
        public string Email { get; set; }
    }

    public record UploadInitialMemberInputDto
    {
        public IFormFile File { get; set; }
    }

    public record UploadInitialMemeberDto
    {
        [Name("First Name")]
        public string FirstName { get; set; }

        [Name("Last Name")]
        public string LastName { get; set; }

        [Name("Email Address")]
        public string Email { get; set; }

        [Name("Honouree Year")]
        public string RecognitionYear { get; set; }

        [Name("MIPAD Category")]
        public string Category { get; set; }
    }

    public record InitialMemberResponseDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Category { get; set; }
        public string Email { get; set; }
        public string RecognitionYear { get; set; }
        public bool IsEmailExist { get; set; }
    }

    public record UploadInitialMemberResponseDto
    {
        public Dictionary<string, string> Errors { get; set; }
        public List<InitialMemberResponseDto> InitialMembers { get; set; }
        public string Message { get; set; }
    }

    public record UploadProfessionalListInputDto
    {
        public IFormFile File { get; set; }
    }

    public record UploadProfessionalListDto
    {
        [Name("Profession")]
        public string Profession { get; set; }

    }
    public record ProfessionalListResponseDto
    {
        public Guid Id { get; set; }
        public string Profession { get; set; }
       
    }
    public record UploadProfessionalListResponseDto
    {
        public Dictionary<string, string> Errors { get; set; }
        public List<ProfessionalListResponseDto> Professions { get; set; }
        public string Message { get; set; }
    }



    public record UploadSubscriptionInputDto
    {
        public IFormFile File { get; set; }
    }

    public record UploadSubscriptionDto
    {
        [Name("SubscriptionPlan")]
        public string SubscriptionPlan { get; set; }

    }
    public record SubscriptionResponseDto
    {
        public Guid Id { get; set; }
        public string SubscriptionPlan { get; set; }

    }
    public record UploadSubscriptionResponseDto
    {
        public Dictionary<string, string> Errors { get; set; }
        public List<SubscriptionResponseDto> SubscriptionPlan { get; set; }
        public string Message { get; set; }
    }


    public record UploadPremiumPlansInputDto
    {
        public IFormFile File { get; set; }
    }

    public record UploadPremiumPlansDto
    {
        [Name("Duration")]
        public string Duration { get; set; }
        [Name("Amount")]
        public decimal Amount { get; set; }
        [Name("TotalAmount")]
        public decimal TotalAmount { get; set; }
        [Name("PercentSavings")]
        public string PercentSavings { get; set; }
        [Name("Savings")]
        public string Savings { get; set; }

    }
    public record PremiumPlansResponseDto
    {
        public Guid PremiumId { get; set; }
        public string Duration { get; set; }
        public decimal Amount { get; set; }
        public decimal TotalAmount { get; set; }
        public string PercentSavings { get; set; }
        public string Savings { get; set; }

    }
    public record UploadPremiumPlansResponseDto
    {
        public Dictionary<string, string> Errors { get; set; }
        public List<PremiumPlansResponseDto> PremiumPlans { get; set; }
        public string Message { get; set; }
    }


    public record UploadMemberGroupInputDto
    {
        public IFormFile File { get; set; }
    }

    public record UploadMemberGroupDto
    {
        [Name("Group")]
        public string Group { get; set; }
        [Name("Description")]
        public string Description { get; set; }

    }
    public record MemberGroupResponseDto
    {
        public Guid GroupId { get; set; }
        public string Group { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    }
    public record UploadMemberGroupResponseDto
    {
        public Dictionary<string, string> Errors { get; set; }
        public List<MemberGroupResponseDto> MemberGroup { get; set; }
        public string Message { get; set; }
    }


}

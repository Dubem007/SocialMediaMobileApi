using Domain.Common;
using Microsoft.AspNetCore.Http;

namespace Shared.DataTransferObjects
{
    public record UserMemberCreationInputDto
    {
        public string ReferenceToken { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PrefferedName { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int RecognitionYear { get; set; }
        public string ProfessionalField { get; set; }
        public IFormFile ProfileImage { get; set; }
        public string Location { get; set; }
        public string Bio { get; set; }
        public string Password { get; set; }

    }

    public record UserMemberUpdateInputDto
    {
        public Guid UserMemberId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PrefferedName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string RecognitionYear { get; set; }
        public string ProfessionalField { get; set; }
        public IFormFile ProfileImage { get; set; }
        public string Location { get; set; }
        public string Bio { get; set; }
    }

    public record UserMemberPhotoUpdateInputDto
    {
        public Guid UserMemberId { get; set; }
        public IFormFile ProfileEditImage { get; set; }
        
    }
    public record UserMemberResponseDto
    {
        public Guid Id { get; set; }
        public Guid UserMemberId { get; set; }
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PrefferedName { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string RecognitionYear { get; set; }
        public string ProfessionalField { get; set; }
        public string ProfileImage { get; set; }
        public string Location { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public string Bio { get; set; }
        public string Country { get; set; }
        public bool IsSubscribed { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string ConnectionStatus { get; set; }
        public IEnumerable<UserConnectionDto> UserConnections { get; set; }
    }

    public class UserConnectionDto
    {
        public Guid MemberId { get; set; }
        public Guid ConnectedMemberId { get; set; }
    }

    public class ConnectStatusDto
    {
        public Guid MemberId { get; set; }
        public string Status { get; set; }
    }

    public class LocationDto
    {
        public float latitude { get; set; }
        public float longitude { get; set; }
    }

    public class LocationDetailsDto
    {
        public List<LocationDto> Locations { get; set; }
    }

    public class BreakdownDto
    {
        public string? Location { get; init; }
        public string Counts { get; init; }
        public float latitude { get; set; }
        public float longitude { get; set; }
        //public List<UserMember>? UserMembers { get; init; }


    }
    public record DeviceTokenCreateDto
    {
     
        public Guid UserId { get; set; }
        public string DeviceId { get; set; }
    }
    public record DeviceTokenDto : DeviceTokenCreateDto
    {
        public Guid Id { get; set; }
    }



}

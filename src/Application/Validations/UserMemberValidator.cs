using FluentValidation;
using Shared.DataTransferObjects;
using Shared.ResourceParameters;

namespace Application.Validations
{
    public class UserMemberValidator : AbstractValidator<UserMemberCreationInputDto>
    {
        public UserMemberValidator()
        {
            RuleFor(x => x.ReferenceToken).NotEmpty();
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
            RuleFor(x => x.DateOfBirth).NotEmpty();
            RuleFor(x => x.RecognitionYear).NotEmpty();
            RuleFor(x => x.ProfessionalField).NotEmpty();
            RuleFor(x => x.Location).NotEmpty();
            RuleFor(x => x.Bio).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
            RuleFor(x => x.ProfileImage).CheckFileFormat(".png, .jpeg, .jpg, .gif").When(x => x.ProfileImage != null);
            RuleFor(x => x.ProfileImage).NotMoreThan40MB().When(x => x.ProfileImage != null);
        }
    }

    public class UserMemberUpdateValidator : AbstractValidator<UserMemberUpdateInputDto>
    {
        public UserMemberUpdateValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
            RuleFor(x => x.PrefferedName).NotEmpty();
            RuleFor(x => x.DateOfBirth).NotEmpty();
            RuleFor(x => x.RecognitionYear).NotEmpty();
            RuleFor(x => x.ProfessionalField).NotEmpty();
            RuleFor(x => x.Location).NotEmpty();
            RuleFor(x => x.Bio).NotEmpty();
        }
    }


    public class UserMemberPhotoUpdateValidator : AbstractValidator<UserMemberPhotoUpdateInputDto>
    {
        public UserMemberPhotoUpdateValidator()
        {
            RuleFor(x => x.UserMemberId).NotEmpty();
            RuleFor(x => x.ProfileEditImage).CheckFileFormat(".png, .jpeg, .jpg, .gif").When(x => x.ProfileEditImage != null);
        }


    }
}
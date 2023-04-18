using FluentValidation;
using Shared.DataTransferObjects;

namespace Application.Validations
{
    public class InitialMemberValidator : AbstractValidator<InitialMemberInputDto>
    {
        public InitialMemberValidator()
        {
            RuleFor(x => x.Email).NotEmpty();
            RuleFor(x => x.Email).EmailAddress();
        }
    }

    public class UploadInitialMemberValidator : AbstractValidator<UploadInitialMemberInputDto>
    {
        public UploadInitialMemberValidator()
        {
            RuleFor(x => x.File).CheckFileFormat(".csv, .xlsx").When(x => x.File != null);
        }
    }
}

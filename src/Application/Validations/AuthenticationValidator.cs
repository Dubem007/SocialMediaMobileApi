using FluentValidation;
using Shared.DataTransferObjects;

namespace Application.Validations
{
    public class ReferenceTokenValidator : AbstractValidator<ReferenceTokenInputDto>
    {
        public ReferenceTokenValidator()
        {
            RuleFor(x => x.ReferenceToken).NotEmpty();
        }
    }

    public class ResetPasswordValidator : AbstractValidator<ResetPasswordDTO>
    {
        public ResetPasswordValidator()
        {
            RuleFor(x => x.ReferenceToken).NotEmpty();
        }
    }

    public class VerifyTokenDtoValidator : AbstractValidator<VerifyTokenDTO>
    {
        public VerifyTokenDtoValidator()
        {
            RuleFor(x => x.Token).NotEmpty();
            RuleFor(x => x.TokenType).NotEmpty();
        }
    }

    public class SendTokenValidator : AbstractValidator<SendTokenInputDto>
    {
        public SendTokenValidator()
        {
            RuleFor(x => x.Email).NotEmpty();
            RuleFor(x => x.TokenType).NotEmpty();
        }
    }

    public class ChangePasswordValidator : AbstractValidator<ChangePasswordDto>
    {
        public ChangePasswordValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.OldPassword).NotEmpty();
            RuleFor(x => x.NewPassword).NotEmpty();
        }
    }
}

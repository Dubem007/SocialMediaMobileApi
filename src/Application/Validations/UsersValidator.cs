using FluentValidation;
using Shared.DataTransferObjects;

namespace Application.Validations
{
    public class UsersValidator : AbstractValidator<CreateUserInputDTO>
    {
        public UsersValidator()
        {

        }
    }
}

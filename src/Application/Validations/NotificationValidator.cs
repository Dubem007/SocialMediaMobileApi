using Domain.Enums;
using FluentValidation;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validations
{
    public class NotificationValidator: AbstractValidator<NotificationCreateDTO>
    {
        public NotificationValidator()
    {
            RuleFor(x => x.Type).IsEnumName(typeof(ENotificationType)).WithMessage(
                    $"This value is not a valid Notification Type.");
        }
}
}

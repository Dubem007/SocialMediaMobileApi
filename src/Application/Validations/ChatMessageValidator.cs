using FluentValidation;
using Shared.DataTransferObjects;

namespace Application.Validations
{
    public class ChatMessageValidator : AbstractValidator<ChatMessageInputDto>
    {
        public ChatMessageValidator()
        {
            RuleFor(x => x.RecipientId).NotEmpty();
            RuleFor(x => x.Text).NotEmpty().When(x => x.MediaTextContent == null);
        }
    }

    public class DeleteChatMessageValidator : AbstractValidator<DeleteChatMessageDto>
    {
        public DeleteChatMessageValidator()
        {
            RuleFor(x => x.RecipientId).NotEmpty();
            RuleFor(x => x.SenderId).NotEmpty();
        }
    }

    public class MediaContentDtoValidator : AbstractValidator<MediaContentDto>
    {
        public MediaContentDtoValidator()
        {
            RuleFor(x => x.Files).CheckFileFormat(".png, .jpeg, .jpg, .gif, .doc, .docx, .pdf, .xlsx, .pptx, .ppt, .rtf").WithMessage("File not in correct format");
            RuleFor(x => x.Files).NotMoreThan40MB().WithMessage("File should not be more than 40MB");
        }
    }
}

namespace Domain.Exceptions;

public sealed class IdsParametersBadRequestException : BadRequestException
{
    public IdsParametersBadRequestException() : base("Parameter ids is null")
    {
    }
}
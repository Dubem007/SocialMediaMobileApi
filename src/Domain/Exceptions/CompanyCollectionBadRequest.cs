namespace Domain.Exceptions;

public class CompanyCollectionBadRequest : BadRequestException
{
    public CompanyCollectionBadRequest() : base("Company collection is null")
    {
    }
}
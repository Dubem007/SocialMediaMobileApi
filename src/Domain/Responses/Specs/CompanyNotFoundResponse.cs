using System;
namespace Domain.Responses.Specs;

public class CompanyNotFoundResponse : ApiNotFoundResponse
{
	public CompanyNotFoundResponse(Guid id) : base($"Company with id {id} not found.")
	{
	}
}


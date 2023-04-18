using System;
namespace Domain.Responses.Specs;

public class EmployeeNotFoundResponse : ApiNotFoundResponse
{
	public EmployeeNotFoundResponse(Guid id) : base($"Employee with id {id} not found.")
	{
	}
}


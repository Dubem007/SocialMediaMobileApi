using System;
namespace Domain.Responses.Specs;

public class UserNotFoundResponse : ApiNotFoundResponse
{
	public UserNotFoundResponse(Guid id) : base($"User with id {id} not found.")
	{
	}
}


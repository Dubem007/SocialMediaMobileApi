using System;
namespace Domain.Responses;

public class ApiNotFoundResponse : ApiBaseResponse
{
	public string Message { get; set; }

	public ApiNotFoundResponse(string message) : base(false)
	{
		Message = message;
	}
}

namespace Domain.Exceptions;

public class EmployeeNotFoundException : NotFoundException
{
    public EmployeeNotFoundException(Guid employeeId) : 
        base($"The employee with id: {employeeId} does not exist")
    {
    }
}
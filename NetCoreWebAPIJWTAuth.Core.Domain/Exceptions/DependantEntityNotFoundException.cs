namespace NetCoreWebAPIJWTAuth.Core.Domain.Exceptions;

public class DependantEntityNotFoundException : NotFoundException
{
    public DependantEntityNotFoundException(Guid employeeId)
        : base($"DependantEntity with id: {employeeId} doesn't exist in the database.")
    {
    }
}

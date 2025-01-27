namespace NetCoreWebAPIJWTAuth.Core.Domain.Exceptions;

public class DependantEntityNotFoundException : NotFoundException
{
    public DependantEntityNotFoundException(Guid dependantEntityId)
        : base($"DependantEntity with id: {dependantEntityId} doesn't exist in the database.")
    {
    }
}

namespace NetCoreWebAPIJWTAuth.Core.Domain.Exceptions;

public sealed class BaseEntityNotFoundException : NotFoundException
{
    public BaseEntityNotFoundException(Guid baseEntityId)
        : base($"The baseEntity with id: {baseEntityId} doesn't exist in the database.")
    {
    }
}
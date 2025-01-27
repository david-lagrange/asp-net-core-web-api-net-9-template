namespace NetCoreWebAPIJWTAuth.Core.Domain.Exceptions;

public sealed class BaseEntityCollectionBadRequest : BadRequestException
{
    public BaseEntityCollectionBadRequest()
        : base("BaseEntity collection sent from a client is null.")
    {
    }
}

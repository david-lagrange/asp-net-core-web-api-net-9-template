namespace NetCoreWebAPIJWTAuth.Core.Domain.Exceptions;

public sealed class RefreshTokenBadRequestException : BadRequestException
{
    public RefreshTokenBadRequestException()
        : base("Refresh token exception. (no user, non-matching, expired)")
    {
    }
}

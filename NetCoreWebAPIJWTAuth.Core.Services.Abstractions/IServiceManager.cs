namespace NetCoreWebAPIJWTAuth.Core.Services.Abstractions;

public interface IServiceManager
{
    IBaseEntityService BaseEntityService { get; }
    IDependantEntityService DependantEntityService { get; }
    IAuthenticationService AuthenticationService { get; }
}

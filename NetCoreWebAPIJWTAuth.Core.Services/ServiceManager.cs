using AutoMapper;
using NetCoreWebAPIJWTAuth.Core.Domain.ConfigurationModels;
using NetCoreWebAPIJWTAuth.Core.Domain.Entities;
using NetCoreWebAPIJWTAuth.Core.Domain.Repositories;
using NetCoreWebAPIJWTAuth.Core.Services.Abstractions;
using LoggingService;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace NetCoreWebAPIJWTAuth.Core.Services;

public sealed class ServiceManager : IServiceManager
{
    private readonly Lazy<IBaseEntityService> _baseEntityService;
    private readonly Lazy<IDependantEntityService> _dependantEntityService;
    private readonly Lazy<IAuthenticationService> _authenticationService;

    public ServiceManager(IRepositoryManager repositoryManager, ILoggerManager logger, IMapper mapper, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IOptions<JwtConfiguration> configuration)
    {
        _authenticationService = new Lazy<IAuthenticationService>(() => new AuthenticationService(logger, mapper, userManager, roleManager, configuration));

        _baseEntityService = new Lazy<IBaseEntityService>(() => new BaseEntityService(repositoryManager, logger, mapper));
        _dependantEntityService = new Lazy<IDependantEntityService>(() => new DependantEntityService(repositoryManager, logger, mapper));
    }

    public IAuthenticationService AuthenticationService => _authenticationService.Value;
    public IBaseEntityService BaseEntityService => _baseEntityService.Value;
    public IDependantEntityService DependantEntityService => _dependantEntityService.Value;
    
}

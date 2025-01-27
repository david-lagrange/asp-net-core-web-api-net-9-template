using NetCoreWebAPIJWTAuth.Core.Domain.Entities;
using Shared.RequestFeatures;

namespace NetCoreWebAPIJWTAuth.Core.Domain.Repositories;

public interface IDependantEntityRepository
{
    Task<PagedList<DependantEntity>> GetDependantEntitiesAsync(Guid baseEntityId, DependantEntityParameters dependantEntityParameters, bool trackChanges, CancellationToken ct = default);
    Task<DependantEntity?> GetDependantEntityAsync(Guid baseEntityId, Guid id, bool trackChanges, CancellationToken ct = default);
    void CreateDependantEntityForBaseEntity(Guid baseEntityId, DependantEntity dependantEntity);
    Task DeleteDependantEntityAsync(BaseEntity baseEntity, DependantEntity dependantEntity, CancellationToken ct = default);
}

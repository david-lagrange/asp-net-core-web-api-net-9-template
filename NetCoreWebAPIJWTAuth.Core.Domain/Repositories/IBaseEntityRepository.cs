using NetCoreWebAPIJWTAuth.Core.Domain.Entities;
using Shared.RequestFeatures;

namespace NetCoreWebAPIJWTAuth.Core.Domain.Repositories;

public interface IBaseEntityRepository
{
    Task<PagedList<BaseEntity>> GetAllBaseEntitiesAsync(BaseEntityParameters baseEntityParameters, bool trackChanges, CancellationToken ct = default);
    Task<BaseEntity?> GetBaseEntityAsync(Guid baseEntityId, bool trackChanges, CancellationToken ct = default);
    void CreateBaseEntity(BaseEntity baseEntity);
    Task<IEnumerable<BaseEntity>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges, CancellationToken ct = default);
    void DeleteBaseEntity(BaseEntity baseEntity);
}

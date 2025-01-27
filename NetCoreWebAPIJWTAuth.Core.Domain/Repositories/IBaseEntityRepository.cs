using NetCoreWebAPIJWTAuth.Core.Domain.Entities;

namespace NetCoreWebAPIJWTAuth.Core.Domain.Repositories;

public interface IBaseEntityRepository
{
    Task<IEnumerable<BaseEntity>> GetAllCompaniesAsync(bool trackChanges, CancellationToken ct = default);
    Task<BaseEntity?> GetBaseEntityAsync(Guid baseEntityId, bool trackChanges, CancellationToken ct = default);
    void CreateBaseEntity(BaseEntity baseEntity);
    Task<IEnumerable<BaseEntity>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges, CancellationToken ct = default);
    void DeleteBaseEntity(BaseEntity baseEntity);
}

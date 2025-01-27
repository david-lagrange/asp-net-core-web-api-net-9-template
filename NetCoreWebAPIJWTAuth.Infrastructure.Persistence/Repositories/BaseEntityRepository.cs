using NetCoreWebAPIJWTAuth.Core.Domain.Repositories;
using NetCoreWebAPIJWTAuth.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace NetCoreWebAPIJWTAuth.Infrastructure.Persistence.Repositories;

internal sealed class BaseEntityRepository : RepositoryBase<BaseEntity>, IBaseEntityRepository
{
    public BaseEntityRepository(RepositoryContext repositoryContext)
        : base(repositoryContext)
    {
    }

    public async Task<IEnumerable<BaseEntity>> GetAllCompaniesAsync(bool trackChanges, CancellationToken ct = default) =>
        await FindAll(trackChanges)
        .OrderBy(c => c.Name)
        .ToListAsync(ct);

    public async Task<BaseEntity?> GetBaseEntityAsync(Guid baseEntityId, bool trackChanges, CancellationToken ct = default) =>
        await FindByCondition(c => c.Id.Equals(baseEntityId), trackChanges)
        .SingleOrDefaultAsync(ct);

    public void CreateBaseEntity(BaseEntity baseEntity) => Create(baseEntity);

    public async Task<IEnumerable<BaseEntity>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges, CancellationToken ct = default) =>
        await FindByCondition(x => ids.Contains(x.Id), trackChanges)
        .ToListAsync(ct);

    public void DeleteBaseEntity(BaseEntity baseEntity) => Delete(baseEntity);
}
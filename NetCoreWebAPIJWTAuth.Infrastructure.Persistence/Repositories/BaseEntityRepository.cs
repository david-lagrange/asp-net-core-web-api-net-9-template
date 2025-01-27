using NetCoreWebAPIJWTAuth.Core.Domain.Repositories;
using NetCoreWebAPIJWTAuth.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.RequestFeatures;
using NetCoreWebAPIJWTAuth.Infrastructure.Persistence.Extensions;

namespace NetCoreWebAPIJWTAuth.Infrastructure.Persistence.Repositories;

internal sealed class BaseEntityRepository : RepositoryBase<BaseEntity>, IBaseEntityRepository
{
    public BaseEntityRepository(RepositoryContext repositoryContext)
        : base(repositoryContext)
    {
    }

    public async Task<PagedList<BaseEntity>> GetAllBaseEntitiesAsync(BaseEntityParameters baseEntityParameters, bool trackChanges, CancellationToken ct = default)
    {
        var baseEntitiesQuery = FindAll(trackChanges)
            .Search(baseEntityParameters.SearchTerm ?? string.Empty)
            .OrderBy(e => e.Name);

        var count = await baseEntitiesQuery.CountAsync(ct);

        var baseEntities = await baseEntitiesQuery
            .Skip((baseEntityParameters.PageNumber - 1) * baseEntityParameters.PageSize)
            .Take(baseEntityParameters.PageSize)
            .ToListAsync(ct);

        return PagedList<BaseEntity>
            .ToPagedList(baseEntities, count, baseEntityParameters.PageNumber,
                baseEntityParameters.PageSize);
    }


    public async Task<BaseEntity?> GetBaseEntityAsync(Guid baseEntityId, bool trackChanges, CancellationToken ct = default) =>
        await FindByCondition(c => c.Id.Equals(baseEntityId), trackChanges)
        .SingleOrDefaultAsync(ct);

    public void CreateBaseEntity(BaseEntity baseEntity) => Create(baseEntity);

    public async Task<IEnumerable<BaseEntity>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges, CancellationToken ct = default) =>
        await FindByCondition(x => ids.Contains(x.Id), trackChanges)
        .ToListAsync(ct);

    public void DeleteBaseEntity(BaseEntity baseEntity) => Delete(baseEntity);
}
using NetCoreWebAPIJWTAuth.Core.Domain.Repositories;
using NetCoreWebAPIJWTAuth.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.RequestFeatures;
using NetCoreWebAPIJWTAuth.Infrastructure.Persistence.Extensions;

namespace NetCoreWebAPIJWTAuth.Infrastructure.Persistence.Repositories;

internal sealed class DependantEntityRepository : RepositoryBase<DependantEntity>, IDependantEntityRepository
{
    public DependantEntityRepository(RepositoryContext repositoryContext)
        : base(repositoryContext)
    {
    }

    public async Task<PagedList<DependantEntity>> GetDependantEntitiesAsync(Guid baseEntityId, DependantEntityParameters dependantEntityParameters, bool trackChanges, CancellationToken ct = default)
    {
        var dependantEntitiesQuery = FindByCondition(e => e.BaseEntityId.Equals(baseEntityId), trackChanges)
            .Search(dependantEntityParameters.SearchTerm ?? string.Empty)
            .OrderBy(e => e.Name);

        var count = await dependantEntitiesQuery.CountAsync(ct);

        var dependantEntities = await dependantEntitiesQuery
            .Skip((dependantEntityParameters.PageNumber - 1) * dependantEntityParameters.PageSize)
            .Take(dependantEntityParameters.PageSize)
            .ToListAsync(ct);

        return PagedList<DependantEntity>
            .ToPagedList(dependantEntities, count, dependantEntityParameters.PageNumber,
                dependantEntityParameters.PageSize);
    }

    public async Task<DependantEntity?> GetDependantEntityAsync(Guid baseEntityId, Guid id, bool trackChanges, CancellationToken ct = default) =>
        await FindByCondition(e => e.BaseEntityId.Equals(baseEntityId) && e.Id.Equals(id), trackChanges)
        .SingleOrDefaultAsync(ct);

    public void CreateDependantEntityForBaseEntity(Guid baseEntityId, DependantEntity dependantEntity)
    {
        dependantEntity.BaseEntityId = baseEntityId;
        Create(dependantEntity);
    }

    public async Task DeleteDependantEntityAsync(BaseEntity baseEntity, DependantEntity dependantEntity, CancellationToken ct = default)
    {
        using var transaction = await RepositoryContext.Database.BeginTransactionAsync(ct);

        Delete(dependantEntity);

        await RepositoryContext.SaveChangesAsync(ct);

        if (!FindByCondition(e => e.BaseEntityId == baseEntity.Id, false).Any())
        {
            RepositoryContext.BaseEntities!.Remove(baseEntity);

            await RepositoryContext.SaveChangesAsync(ct);
        }

        await transaction.CommitAsync(ct);
    }
}

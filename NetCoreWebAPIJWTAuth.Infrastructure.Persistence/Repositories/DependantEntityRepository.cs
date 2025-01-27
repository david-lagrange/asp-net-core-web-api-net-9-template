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

    public async Task<PagedList<DependantEntity>> GetDependantEntitysAsync(Guid baseEntityId, DependantEntityParameters employeeParameters, bool trackChanges, CancellationToken ct = default)
    {
        var employeesQuery = FindByCondition(e => e.BaseEntityId.Equals(baseEntityId), trackChanges)
            .FilterDependantEntitys(employeeParameters.MinAge, employeeParameters.MaxAge)
            .Search(employeeParameters.SearchTerm ?? string.Empty)
            .OrderBy(e => e.Name);

        var count = await employeesQuery.CountAsync(ct);

        var employees = await employeesQuery
            .Skip((employeeParameters.PageNumber - 1) * employeeParameters.PageSize)
            .Take(employeeParameters.PageSize)
            .ToListAsync(ct);

        return PagedList<DependantEntity>
            .ToPagedList(employees, count, employeeParameters.PageNumber,
                employeeParameters.PageSize);
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
            RepositoryContext.Companies!.Remove(baseEntity);

            await RepositoryContext.SaveChangesAsync(ct);
        }

        await transaction.CommitAsync(ct);
    }
}

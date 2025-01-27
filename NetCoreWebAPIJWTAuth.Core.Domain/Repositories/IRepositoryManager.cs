using Microsoft.EntityFrameworkCore.Storage;

namespace NetCoreWebAPIJWTAuth.Core.Domain.Repositories;

public interface IRepositoryManager
{
    IBaseEntityRepository BaseEntity { get; }
    IDependantEntityRepository DependantEntity { get; }
    Task SaveAsync(CancellationToken ct = default);
    IDbContextTransaction BeginTransaction();
}

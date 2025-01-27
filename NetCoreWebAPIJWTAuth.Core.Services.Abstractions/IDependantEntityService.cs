using NetCoreWebAPIJWTAuth.Core.Domain.Entities;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;

namespace NetCoreWebAPIJWTAuth.Core.Services.Abstractions;

public interface IDependantEntityService
{
    Task<(IEnumerable<DependantEntityDto> employees, MetaData metaData)> GetDependantEntitysAsync(Guid baseEntityId, DependantEntityParameters employeeParameters, bool trackChanges, CancellationToken ct = default);
    Task<DependantEntityDto> GetDependantEntityAsync(Guid baseEntityId, Guid id, bool trackChanges, CancellationToken ct = default);
    Task<DependantEntityDto> CreateDependantEntityForBaseEntityAsync(Guid baseEntityId, DependantEntityForCreationDto employeeForCreation, bool trackChanges, CancellationToken ct = default);
    Task DeleteDependantEntityForBaseEntityAsync(Guid baseEntityId, Guid id, bool trackChanges, CancellationToken ct = default);
    Task UpdateDependantEntityForBaseEntityAsync(Guid baseEntityId, Guid id, DependantEntityForUpdateDto employeeForUpdate, bool compTrackChanges, bool empTrackChanges, CancellationToken ct = default);
    Task<(DependantEntityForUpdateDto employeeToPatch, DependantEntity employeeEntity)> GetDependantEntityForPatchAsync(Guid baseEntityId, Guid id, bool compTrackChanges, bool empTrackChanges, CancellationToken ct = default);
    Task SaveChangesForPatchAsync(DependantEntityForUpdateDto employeeToPatch, DependantEntity employeeEntity, CancellationToken ct = default);
}
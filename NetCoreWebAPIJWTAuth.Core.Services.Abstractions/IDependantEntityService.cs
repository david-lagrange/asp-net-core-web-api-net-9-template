using NetCoreWebAPIJWTAuth.Core.Domain.Entities;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;

namespace NetCoreWebAPIJWTAuth.Core.Services.Abstractions;

public interface IDependantEntityService
{
    Task<(IEnumerable<DependantEntityDto> dependantEntities, MetaData metaData)> GetDependantEntitiesAsync(Guid baseEntityId, DependantEntityParameters dependantEntityParameters, bool trackChanges, CancellationToken ct = default);
    Task<DependantEntityDto> GetDependantEntityAsync(Guid baseEntityId, Guid id, bool trackChanges, CancellationToken ct = default);
    Task<DependantEntityDto> CreateDependantEntityForBaseEntityAsync(Guid baseEntityId, DependantEntityForCreationDto dependantEntityForCreation, bool trackChanges, CancellationToken ct = default);
    Task DeleteDependantEntityForBaseEntityAsync(Guid baseEntityId, Guid id, bool trackChanges, CancellationToken ct = default);
    Task UpdateDependantEntityForBaseEntityAsync(Guid baseEntityId, Guid id, DependantEntityForUpdateDto dependantEntityForUpdate, bool compTrackChanges, bool empTrackChanges, CancellationToken ct = default);
    Task<(DependantEntityForUpdateDto dependantEntityToPatch, DependantEntity dependantEntityEntity)> GetDependantEntityForPatchAsync(Guid baseEntityId, Guid id, bool compTrackChanges, bool empTrackChanges, CancellationToken ct = default);
    Task SaveChangesForPatchAsync(DependantEntityForUpdateDto dependantEntityToPatch, DependantEntity dependantEntityEntity, CancellationToken ct = default);
}
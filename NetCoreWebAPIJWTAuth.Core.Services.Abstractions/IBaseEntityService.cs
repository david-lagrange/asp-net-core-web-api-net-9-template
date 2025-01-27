using Shared.DataTransferObjects;
using Shared.RequestFeatures;

namespace NetCoreWebAPIJWTAuth.Core.Services.Abstractions;

public interface IBaseEntityService
{
    Task<IEnumerable<BaseEntityDto>> GetAllBaseEntitiesAsync(BaseEntityParameters baseEntityParameters, bool trackChanges, CancellationToken ct = default);
    Task<BaseEntityDto> GetBaseEntityAsync(Guid baseEntityId, bool trackChanges, CancellationToken ct = default);
    Task<BaseEntityDto> CreateBaseEntityAsync(BaseEntityForCreationDto baseEntity, CancellationToken ct = default);
    Task<IEnumerable<BaseEntityDto>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges, CancellationToken ct = default);
    Task<(IEnumerable<BaseEntityDto> baseEntities, string ids)> CreateBaseEntityCollectionAsync(IEnumerable<BaseEntityForCreationDto> baseEntityCollection, CancellationToken ct = default);
    Task DeleteBaseEntityAsync(Guid baseEntityId, bool trackChanges, CancellationToken ct = default);
    Task UpdateBaseEntityAsync(Guid baseEntityid, BaseEntityForUpdateDto baseEntityForUpdate, bool trackChanges, CancellationToken ct = default);
}
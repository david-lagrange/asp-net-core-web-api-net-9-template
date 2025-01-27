using AutoMapper;
using NetCoreWebAPIJWTAuth.Core.Domain.Entities;
using NetCoreWebAPIJWTAuth.Core.Domain.Exceptions;
using NetCoreWebAPIJWTAuth.Core.Domain.Repositories;
using NetCoreWebAPIJWTAuth.Core.Services.Abstractions;
using LoggingService;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;

namespace NetCoreWebAPIJWTAuth.Core.Services;

internal sealed class BaseEntityService : IBaseEntityService
{
    private readonly IRepositoryManager _repository;
    private readonly ILoggerManager _logger;
    private readonly IMapper _mapper;

    public BaseEntityService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<IEnumerable<BaseEntityDto>> GetAllBaseEntitiesAsync(BaseEntityParameters baseEntityParameters, bool trackChanges, CancellationToken ct = default)
    {
        var baseEntities = await _repository.BaseEntity.GetAllBaseEntitiesAsync(baseEntityParameters, trackChanges, ct);

        var baseEntitiesDto = _mapper.Map<IEnumerable<BaseEntityDto>>(baseEntities);

        return baseEntitiesDto;
    }

    public async Task<BaseEntityDto> GetBaseEntityAsync(Guid id, bool trackChanges, CancellationToken ct = default)
    {
        var baseEntity = await GetBaseEntityAndCheckIfItExists(id, trackChanges, ct);

        var baseEntityDto = _mapper.Map<BaseEntityDto>(baseEntity);

        return baseEntityDto;
    }

    public async Task<BaseEntityDto> CreateBaseEntityAsync(BaseEntityForCreationDto baseEntity, CancellationToken ct = default)
    {
        var baseEntityEntity = _mapper.Map<BaseEntity>(baseEntity);

        _repository.BaseEntity.CreateBaseEntity(baseEntityEntity);
        await _repository.SaveAsync(ct);

        var baseEntityToReturn = _mapper.Map<BaseEntityDto>(baseEntityEntity);

        return baseEntityToReturn;
    }

    public async Task<IEnumerable<BaseEntityDto>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges, CancellationToken ct = default)
    {
        if (ids is null)
            throw new IdParametersBadRequestException();

        var baseEntityEntities = await _repository.BaseEntity.GetByIdsAsync(ids, trackChanges, ct);

        if (ids.Count() != baseEntityEntities.Count())
            throw new CollectionByIdsBadRequestException();

        var baseEntitiesToReturn = _mapper.Map<IEnumerable<BaseEntityDto>>(baseEntityEntities);

        return baseEntitiesToReturn;
    }

    public async Task<(IEnumerable<BaseEntityDto> baseEntities, string ids)> CreateBaseEntityCollectionAsync(IEnumerable<BaseEntityForCreationDto> baseEntityCollection, CancellationToken ct = default)
    {
        if (baseEntityCollection is null)
            throw new BaseEntityCollectionBadRequest();

        var baseEntityEntities = _mapper.Map<IEnumerable<BaseEntity>>(baseEntityCollection);
        foreach (var baseEntity in baseEntityEntities)
        {
            _repository.BaseEntity.CreateBaseEntity(baseEntity);
        }

        await _repository.SaveAsync(ct);

        var baseEntityCollectionToReturn = _mapper.Map<IEnumerable<BaseEntityDto>>(baseEntityEntities);
        var ids = string.Join(",", baseEntityCollectionToReturn.Select(c => c.Id));

        return (baseEntities: baseEntityCollectionToReturn, ids: ids);
    }


    public async Task DeleteBaseEntityAsync(Guid baseEntityId, bool trackChanges, CancellationToken ct = default)
    {
        var baseEntity = await GetBaseEntityAndCheckIfItExists(baseEntityId, trackChanges, ct);

        _repository.BaseEntity.DeleteBaseEntity(baseEntity);
        await _repository.SaveAsync(ct);
    }

    public async Task UpdateBaseEntityAsync(Guid baseEntityId, BaseEntityForUpdateDto baseEntityForUpdate,
        bool trackChanges, CancellationToken ct = default)
    {
        var baseEntity = await GetBaseEntityAndCheckIfItExists(baseEntityId, trackChanges, ct);

        _mapper.Map(baseEntityForUpdate, baseEntity);
        await _repository.SaveAsync(ct);
    }

    private async Task<BaseEntity> GetBaseEntityAndCheckIfItExists(Guid id, bool trackChanges, CancellationToken ct)
    {
        var baseEntity = await _repository.BaseEntity.GetBaseEntityAsync(id, trackChanges, ct);
        if (baseEntity is null)
            throw new BaseEntityNotFoundException(id);

        return baseEntity;
    }
}

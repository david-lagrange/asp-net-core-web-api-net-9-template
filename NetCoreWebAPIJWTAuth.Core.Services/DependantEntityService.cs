using AutoMapper;
using NetCoreWebAPIJWTAuth.Core.Domain.Entities;
using NetCoreWebAPIJWTAuth.Core.Domain.Exceptions;
using NetCoreWebAPIJWTAuth.Core.Domain.Repositories;
using NetCoreWebAPIJWTAuth.Core.Services.Abstractions;
using LoggingService;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;

namespace NetCoreWebAPIJWTAuth.Core.Services;

internal sealed class DependantEntityService : IDependantEntityService
{
    private readonly IRepositoryManager _repository;
    private readonly ILoggerManager _logger;
    private readonly IMapper _mapper;

    public DependantEntityService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<(IEnumerable<DependantEntityDto> dependantEntities, MetaData metaData)> GetDependantEntitiesAsync(Guid baseEntityId, DependantEntityParameters dependantEntityParameters, bool trackChanges, CancellationToken ct = default)
    {
        await CheckIfBaseEntityExists(baseEntityId, trackChanges, ct);

        var dependantEntitiesWithMetaData = await _repository.DependantEntity
            .GetDependantEntitiesAsync(baseEntityId, dependantEntityParameters, trackChanges, ct);

        var dependantEntitiesDto = _mapper.Map<IEnumerable<DependantEntityDto>>(dependantEntitiesWithMetaData);

        return (dependantEntities: dependantEntitiesDto, metaData: dependantEntitiesWithMetaData.MetaData);
    }

    public async Task<DependantEntityDto> GetDependantEntityAsync(Guid baseEntityId, Guid id, bool trackChanges, CancellationToken ct = default)
    {
        await CheckIfBaseEntityExists(baseEntityId, trackChanges, ct);

        var dependantEntityDb = await _repository.DependantEntity.GetDependantEntityAsync(baseEntityId, id, trackChanges, ct);
        if (dependantEntityDb is null)
            throw new DependantEntityNotFoundException(id);

        var dependantEntity = _mapper.Map<DependantEntityDto>(dependantEntityDb);
        return dependantEntity;
    }

    public async Task<DependantEntityDto> CreateDependantEntityForBaseEntityAsync(Guid baseEntityId, DependantEntityForCreationDto dependantEntityForCreation, bool trackChanges, CancellationToken ct = default)
    {
        await CheckIfBaseEntityExists(baseEntityId, trackChanges, ct);

        var dependantEntityEntity = _mapper.Map<DependantEntity>(dependantEntityForCreation);

        _repository.DependantEntity.CreateDependantEntityForBaseEntity(baseEntityId, dependantEntityEntity);
        await _repository.SaveAsync(ct);

        var dependantEntityToReturn = _mapper.Map<DependantEntityDto>(dependantEntityEntity);

        return dependantEntityToReturn;
    }

    public async Task DeleteDependantEntityForBaseEntityAsync(Guid baseEntityId, Guid id, bool trackChanges, CancellationToken ct = default)
    {
        var baseEntity = await CheckIfBaseEntityExists(baseEntityId, trackChanges, ct);

        var dependantEntityDb = await GetDependantEntityForBaseEntityAndCheckIfItExists(baseEntityId, id,
            trackChanges, ct);

        await _repository.DependantEntity.DeleteDependantEntityAsync(baseEntity, dependantEntityDb, ct);

        await _repository.SaveAsync();
    }

    public async Task UpdateDependantEntityForBaseEntityAsync(Guid baseEntityId, Guid id, DependantEntityForUpdateDto dependantEntityForUpdate, bool compTrackChanges, bool empTrackChanges, CancellationToken ct = default)
    {
        await CheckIfBaseEntityExists(baseEntityId, compTrackChanges, ct);

        var dependantEntityDb = await GetDependantEntityForBaseEntityAndCheckIfItExists(baseEntityId, id, empTrackChanges, ct);

        _mapper.Map(dependantEntityForUpdate, dependantEntityDb);

        await _repository.SaveAsync();
    }

    public async Task<(DependantEntityForUpdateDto dependantEntityToPatch, DependantEntity dependantEntityEntity)> GetDependantEntityForPatchAsync(Guid baseEntityId, Guid id, bool compTrackChanges, bool empTrackChanges, CancellationToken ct)
    {
        await CheckIfBaseEntityExists(baseEntityId, compTrackChanges, ct);

        var dependantEntityDb = await GetDependantEntityForBaseEntityAndCheckIfItExists(baseEntityId, id, empTrackChanges, ct);

        var dependantEntityToPatch = _mapper.Map<DependantEntityForUpdateDto>(dependantEntityDb);

        return (dependantEntityToPatch, dependantEntityDb);
    }

    public async Task SaveChangesForPatchAsync(DependantEntityForUpdateDto dependantEntityToPatch, DependantEntity dependantEntityEntity, CancellationToken ct = default)
    {
        _mapper.Map(dependantEntityToPatch, dependantEntityEntity);

        await _repository.SaveAsync(ct);
    }

    private async Task<BaseEntity> CheckIfBaseEntityExists(Guid baseEntityId, bool trackChanges, CancellationToken ct)
    {
        var baseEntity = await _repository.BaseEntity.GetBaseEntityAsync(baseEntityId, trackChanges, ct);
        if (baseEntity is null)
            throw new BaseEntityNotFoundException(baseEntityId);

        return baseEntity;
    }

    private async Task<DependantEntity> GetDependantEntityForBaseEntityAndCheckIfItExists(Guid baseEntityId,
        Guid id, bool trackChanges, CancellationToken ct)
    {
        var dependantEntityDb = await _repository.DependantEntity.GetDependantEntityAsync(baseEntityId, id, trackChanges, ct);
        if (dependantEntityDb is null)
            throw new DependantEntityNotFoundException(id);
        return dependantEntityDb;
    }
}

//public void DeleteDependantEntityForBaseEntity(Guid baseEntityId, Guid id, bool trackChanges)
//{
//    var baseEntity = _repository.BaseEntity.GetBaseEntity(baseEntityId, trackChanges);
//    if (baseEntity is null)
//        throw new BaseEntityNotFoundException(baseEntityId);

//    var dependantEntityForBaseEntity = _repository.DependantEntity.GetDependantEntity(baseEntityId, id, trackChanges);
//    if (dependantEntityForBaseEntity is null)
//        throw new DependantEntityNotFoundException(id);

//    using var trans = _repository.BeginTransaction();

//    _repository.DependantEntity.DeleteDependantEntity(baseEntity, dependantEntityForBaseEntity);

//    trans.Commit();
//}
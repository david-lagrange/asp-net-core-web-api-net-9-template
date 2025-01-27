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

    public async Task<(IEnumerable<DependantEntityDto> employees, MetaData metaData)> GetDependantEntitysAsync(Guid baseEntityId, DependantEntityParameters employeeParameters, bool trackChanges, CancellationToken ct = default)
    {
        if (!employeeParameters.ValidAgeRange)
            throw new MaxAgeRangeBadRequestException();

        await CheckIfBaseEntityExists(baseEntityId, trackChanges, ct);

        var employeesWithMetaData = await _repository.DependantEntity
            .GetDependantEntitysAsync(baseEntityId, employeeParameters, trackChanges, ct);

        var employeesDto = _mapper.Map<IEnumerable<DependantEntityDto>>(employeesWithMetaData);

        return (employees: employeesDto, metaData: employeesWithMetaData.MetaData);
    }

    public async Task<DependantEntityDto> GetDependantEntityAsync(Guid baseEntityId, Guid id, bool trackChanges, CancellationToken ct = default)
    {
        await CheckIfBaseEntityExists(baseEntityId, trackChanges, ct);

        var employeeDb = await _repository.DependantEntity.GetDependantEntityAsync(baseEntityId, id, trackChanges, ct);
        if (employeeDb is null)
            throw new DependantEntityNotFoundException(id);

        var dependantEntity = _mapper.Map<DependantEntityDto>(employeeDb);
        return dependantEntity;
    }

    public async Task<DependantEntityDto> CreateDependantEntityForBaseEntityAsync(Guid baseEntityId, DependantEntityForCreationDto employeeForCreation, bool trackChanges, CancellationToken ct = default)
    {
        await CheckIfBaseEntityExists(baseEntityId, trackChanges, ct);

        var employeeEntity = _mapper.Map<DependantEntity>(employeeForCreation);

        _repository.DependantEntity.CreateDependantEntityForBaseEntity(baseEntityId, employeeEntity);
        await _repository.SaveAsync(ct);

        var employeeToReturn = _mapper.Map<DependantEntityDto>(employeeEntity);

        return employeeToReturn;
    }

    public async Task DeleteDependantEntityForBaseEntityAsync(Guid baseEntityId, Guid id, bool trackChanges, CancellationToken ct = default)
    {
        var baseEntity = await CheckIfBaseEntityExists(baseEntityId, trackChanges, ct);

        var employeeDb = await GetDependantEntityForBaseEntityAndCheckIfItExists(baseEntityId, id,
            trackChanges, ct);

        await _repository.DependantEntity.DeleteDependantEntityAsync(baseEntity, employeeDb, ct);

        await _repository.SaveAsync();
    }

    public async Task UpdateDependantEntityForBaseEntityAsync(Guid baseEntityId, Guid id, DependantEntityForUpdateDto employeeForUpdate, bool compTrackChanges, bool empTrackChanges, CancellationToken ct = default)
    {
        await CheckIfBaseEntityExists(baseEntityId, compTrackChanges, ct);

        var employeeDb = await GetDependantEntityForBaseEntityAndCheckIfItExists(baseEntityId, id, empTrackChanges, ct);

        _mapper.Map(employeeForUpdate, employeeDb);

        await _repository.SaveAsync();
    }

    public async Task<(DependantEntityForUpdateDto employeeToPatch, DependantEntity employeeEntity)> GetDependantEntityForPatchAsync(Guid baseEntityId, Guid id, bool compTrackChanges, bool empTrackChanges, CancellationToken ct)
    {
        await CheckIfBaseEntityExists(baseEntityId, compTrackChanges, ct);

        var employeeDb = await GetDependantEntityForBaseEntityAndCheckIfItExists(baseEntityId, id, empTrackChanges, ct);

        var employeeToPatch = _mapper.Map<DependantEntityForUpdateDto>(employeeDb);

        return (employeeToPatch, employeeDb);
    }

    public async Task SaveChangesForPatchAsync(DependantEntityForUpdateDto employeeToPatch, DependantEntity employeeEntity, CancellationToken ct = default)
    {
        _mapper.Map(employeeToPatch, employeeEntity);

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
        var employeeDb = await _repository.DependantEntity.GetDependantEntityAsync(baseEntityId, id, trackChanges, ct);
        if (employeeDb is null)
            throw new DependantEntityNotFoundException(id);
        return employeeDb;
    }
}

//public void DeleteDependantEntityForBaseEntity(Guid baseEntityId, Guid id, bool trackChanges)
//{
//    var baseEntity = _repository.BaseEntity.GetBaseEntity(baseEntityId, trackChanges);
//    if (baseEntity is null)
//        throw new BaseEntityNotFoundException(baseEntityId);

//    var employeeForBaseEntity = _repository.DependantEntity.GetDependantEntity(baseEntityId, id, trackChanges);
//    if (employeeForBaseEntity is null)
//        throw new DependantEntityNotFoundException(id);

//    using var trans = _repository.BeginTransaction();

//    _repository.DependantEntity.DeleteDependantEntity(baseEntity, employeeForBaseEntity);

//    trans.Commit();
//}
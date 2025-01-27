using NetCoreWebAPIJWTAuth.Core.Services.Abstractions;
using NetCoreWebAPIJWTAuth.Infrastructure.Presentation.ActionFilters;
using NetCoreWebAPIJWTAuth.Infrastructure.Presentation.ModelBinders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;

namespace NetCoreWebAPIJWTAuth.Infrastructure.Presentation.Controllers;

[Route("api/baseEntities")]
[ApiController]
public class BaseEntitiesController : ControllerBase
{
    private readonly IServiceManager _service;

    public BaseEntitiesController(IServiceManager service) => _service = service;

    [HttpOptions]
    public IActionResult GetBaseEntitiesOptions()
    {
        Response.Headers.Append("Allow", "GET, OPTIONS, POST, PUT, DELETE");

        return Ok();
    }

    [HttpGet]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> GetBaseEntities([FromQuery] BaseEntityParameters baseEntityParameters, CancellationToken ct)
    {
        var baseEntities = await _service.BaseEntityService.GetAllBaseEntitiesAsync(baseEntityParameters, trackChanges: false, ct);

        return Ok(baseEntities);
    }

    [HttpGet("{id:guid}", Name = "BaseEntityById")]
    public async Task<IActionResult> GetBaseEntity(Guid id, CancellationToken ct)
    {
        var baseEntity = await _service.BaseEntityService.GetBaseEntityAsync(id, trackChanges: false, ct);

        return Ok(baseEntity);
    }

    [HttpPost(Name = "CreateBaseEntity")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> CreateBaseEntity([FromBody] BaseEntityForCreationDto baseEntity, CancellationToken ct)
    {
        var createdBaseEntity = await _service.BaseEntityService.CreateBaseEntityAsync(baseEntity, ct);

        return CreatedAtRoute("BaseEntityById", new { id = createdBaseEntity.Id }, createdBaseEntity);
    }

    [HttpGet("collection/({ids})", Name = "BaseEntityCollection")]
    public async Task<IActionResult> GetBaseEntityCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids, CancellationToken ct)
    {
        var baseEntities = await _service.BaseEntityService.GetByIdsAsync(ids, trackChanges: false, ct);

        return Ok(baseEntities);
    }

    [HttpPost("collection")]
    public async Task<IActionResult> CreateBaseEntityCollection([FromBody] IEnumerable<BaseEntityForCreationDto> baseEntityCollection, CancellationToken ct)
    {
        var result = await _service.BaseEntityService.CreateBaseEntityCollectionAsync(baseEntityCollection, ct);

        return CreatedAtRoute("BaseEntityCollection", new { result.ids }, result.baseEntities);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteBaseEntity(Guid id, CancellationToken ct)
    {
        await _service.BaseEntityService.DeleteBaseEntityAsync(id, trackChanges: false, ct);

        return NoContent();
    }

    [HttpPut("{id:guid}")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> UpdateBaseEntity(Guid id, [FromBody] BaseEntityForUpdateDto baseEntity, CancellationToken ct)
    {
        await _service.BaseEntityService.UpdateBaseEntityAsync(id, baseEntity, trackChanges: true, ct);

        return NoContent();
    }
}
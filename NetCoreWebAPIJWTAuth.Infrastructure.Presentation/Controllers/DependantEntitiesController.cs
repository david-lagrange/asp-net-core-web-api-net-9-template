using NetCoreWebAPIJWTAuth.Core.Services.Abstractions;
using NetCoreWebAPIJWTAuth.Infrastructure.Presentation.ActionFilters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;
using System.Text.Json;

namespace NetCoreWebAPIJWTAuth.Infrastructure.Presentation.Controllers;

[Route("api/baseEntities/{baseEntityId}/dependantEntities")]
[ApiController]
public class DependantEntitiesController : ControllerBase
{
    private readonly IServiceManager _service;

    public DependantEntitiesController(IServiceManager service) => _service = service;

    [HttpGet]
    [HttpHead]
    public async Task<IActionResult> GetDependantEntitiesForBaseEntity(Guid baseEntityId, [FromQuery] DependantEntityParameters dependantEntityParameters, CancellationToken ct)
    {
        var pagedResult = await _service.DependantEntityService.GetDependantEntitiesAsync(baseEntityId,
        dependantEntityParameters, trackChanges: false, ct);

        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(pagedResult.metaData));

        return Ok(pagedResult.dependantEntities);
    }

    [HttpGet("{id:guid}", Name = "GetDependantEntityForBaseEntity")]
    public async Task<IActionResult> GetDependantEntityForBaseEntity(Guid baseEntityId, Guid id, CancellationToken ct)
    {
        var dependantEntity = await _service.DependantEntityService.GetDependantEntityAsync(baseEntityId, id, trackChanges: false, ct);

        return Ok(dependantEntity);
    }

    [HttpPost]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> CreateDependantEntityForBaseEntity(Guid baseEntityId, [FromBody] DependantEntityForCreationDto dependantEntity, CancellationToken ct)
    {
        var dependantEntityToReturn = await _service.DependantEntityService.CreateDependantEntityForBaseEntityAsync(baseEntityId, dependantEntity, trackChanges: false, ct);

        return CreatedAtRoute("GetDependantEntityForBaseEntity", new { baseEntityId, id = dependantEntityToReturn.Id },
            dependantEntityToReturn);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteDependantEntityForBaseEntity(Guid baseEntityId, Guid id, CancellationToken ct)
    {
        await _service.DependantEntityService.DeleteDependantEntityForBaseEntityAsync(baseEntityId, id, trackChanges: false, ct);

        return NoContent();
    }

    [HttpPut("{id:guid}")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> UpdateDependantEntityForBaseEntity(Guid baseEntityId, Guid id, [FromBody] DependantEntityForUpdateDto dependantEntity, CancellationToken ct)
    { 
        await _service.DependantEntityService.UpdateDependantEntityForBaseEntityAsync(baseEntityId, id, dependantEntity, compTrackChanges: false, empTrackChanges: true, ct);

        return NoContent();
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> PartiallyUpdateDependantEntityForBaseEntity(Guid baseEntityId, Guid id, [FromBody] JsonPatchDocument<DependantEntityForUpdateDto> patchDoc, CancellationToken ct)
    {
        if (patchDoc is null)
            return BadRequest("patchDoc object sent from client is null.");

        var result = await _service.DependantEntityService.GetDependantEntityForPatchAsync(baseEntityId, id,
            compTrackChanges: false, empTrackChanges: true, ct);

        patchDoc.ApplyTo(result.dependantEntityToPatch, ModelState);

        TryValidateModel(result.dependantEntityToPatch);

        if (!ModelState.IsValid)
            return UnprocessableEntity(ModelState);

        await _service.DependantEntityService.SaveChangesForPatchAsync(result.dependantEntityToPatch, result.dependantEntityEntity, ct);

        return NoContent();
    }
}
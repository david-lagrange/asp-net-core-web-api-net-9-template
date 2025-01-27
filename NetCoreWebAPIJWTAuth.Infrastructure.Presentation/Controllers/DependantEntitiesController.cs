using NetCoreWebAPIJWTAuth.Core.Services.Abstractions;
using NetCoreWebAPIJWTAuth.Infrastructure.Presentation.ActionFilters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;
using System.Text.Json;

namespace NetCoreWebAPIJWTAuth.Infrastructure.Presentation.Controllers;

[Route("api/companies/{baseEntityId}/employees")]
[ApiController]
public class DependantEntitysController : ControllerBase
{
    private readonly IServiceManager _service;

    public DependantEntitysController(IServiceManager service) => _service = service;

    [HttpGet]
    [HttpHead]
    public async Task<IActionResult> GetDependantEntitysForBaseEntity(Guid baseEntityId, [FromQuery] DependantEntityParameters employeeParameters, CancellationToken ct)
    {
        var pagedResult = await _service.DependantEntityService.GetDependantEntitysAsync(baseEntityId,
        employeeParameters, trackChanges: false, ct);

        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(pagedResult.metaData));

        return Ok(pagedResult.employees);
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
        var employeeToReturn = await _service.DependantEntityService.CreateDependantEntityForBaseEntityAsync(baseEntityId, dependantEntity, trackChanges: false, ct);

        return CreatedAtRoute("GetDependantEntityForBaseEntity", new { baseEntityId, id = employeeToReturn.Id },
            employeeToReturn);
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

        patchDoc.ApplyTo(result.employeeToPatch, ModelState);

        TryValidateModel(result.employeeToPatch);

        if (!ModelState.IsValid)
            return UnprocessableEntity(ModelState);

        await _service.DependantEntityService.SaveChangesForPatchAsync(result.employeeToPatch, result.employeeEntity, ct);

        return NoContent();
    }
}
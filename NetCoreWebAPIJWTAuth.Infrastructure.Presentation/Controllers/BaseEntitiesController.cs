using NetCoreWebAPIJWTAuth.Core.Services.Abstractions;
using NetCoreWebAPIJWTAuth.Infrastructure.Presentation.ActionFilters;
using NetCoreWebAPIJWTAuth.Infrastructure.Presentation.ModelBinders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.DataTransferObjects;

namespace NetCoreWebAPIJWTAuth.Infrastructure.Presentation.Controllers;

[Route("api/companies")]
[ApiController]
public class BaseEntitiesController : ControllerBase
{
    private readonly IServiceManager _service;

    public BaseEntitiesController(IServiceManager service) => _service = service;

    [HttpOptions]
    public IActionResult GetCompaniesOptions()
    {
        Response.Headers.Append("Allow", "GET, OPTIONS, POST, PUT, DELETE");

        return Ok();
    }

    /// <summary>
    /// Gets the list of all companies
    /// </summary>
    /// <returns>The companies list</returns>
    [HttpGet]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> GetCompanies(CancellationToken ct)
    {
        var companies = await _service.BaseEntityService.GetAllCompaniesAsync(trackChanges: false, ct);

        return Ok(companies);
    }

    [HttpGet("{id:guid}", Name = "BaseEntityById")]
    public async Task<IActionResult> GetBaseEntity(Guid id, CancellationToken ct)
    {
        var baseEntity = await _service.BaseEntityService.GetBaseEntityAsync(id, trackChanges: false, ct);

        return Ok(baseEntity);
    }

    /// <summary>
    /// Creates a newly created baseEntity
    /// </summary>
    /// <param name="baseEntity"></param>
    /// <returns>A newly created baseEntity</returns>
    /// <response code="201">Returns the newly created item</response>
    /// <response code="400">If the item is null</response>
    /// <response code="422">If the model is invalid</response>
    [HttpPost(Name = "CreateBaseEntity")]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(422)]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> CreateBaseEntity([FromBody] BaseEntityForCreationDto baseEntity, CancellationToken ct)
    {
        var createdBaseEntity = await _service.BaseEntityService.CreateBaseEntityAsync(baseEntity, ct);

        return CreatedAtRoute("BaseEntityById", new { id = createdBaseEntity.Id }, createdBaseEntity);
    }

    [HttpGet("collection/({ids})", Name = "BaseEntityCollection")]
    public async Task<IActionResult> GetBaseEntityCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids, CancellationToken ct)
    {
        var companies = await _service.BaseEntityService.GetByIdsAsync(ids, trackChanges: false, ct);

        return Ok(companies);
    }

    [HttpPost("collection")]
    public async Task<IActionResult> CreateBaseEntityCollection([FromBody] IEnumerable<BaseEntityForCreationDto> baseEntityCollection, CancellationToken ct)
    {
        var result = await _service.BaseEntityService.CreateBaseEntityCollectionAsync(baseEntityCollection, ct);

        return CreatedAtRoute("BaseEntityCollection", new { result.ids }, result.companies);
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
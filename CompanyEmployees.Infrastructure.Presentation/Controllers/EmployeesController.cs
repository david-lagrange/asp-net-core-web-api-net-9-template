using CompanyEmployees.Core.Services.Abstractions;
using CompanyEmployees.Infrastructure.Presentation.ActionFilters;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Shared.DataTransferObjects;

namespace CompanyEmployees.Infrastructure.Presentation.Controllers;

[Route("api/companies/{companyId}/employees")]
[ApiController]
public class EmployeesController : ControllerBase
{
    private readonly IServiceManager _service;

    public EmployeesController(IServiceManager service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetEmployeesForCompany(Guid companyId, CancellationToken ct)
    {
        var employees = await _service.EmployeeService.GetEmployeesAsync(companyId, trackChanges: false, ct);

        return Ok(employees);
    }

    [HttpGet("{id:guid}", Name = "GetEmployeeForCompany")]
    public async Task<IActionResult> GetEmployeeForCompany(Guid companyId, Guid id, CancellationToken ct)
    {
        var employee = await _service.EmployeeService.GetEmployeeAsync(companyId, id, trackChanges: false, ct);

        return Ok(employee);
    }

    [HttpPost]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> CreateEmployeeForCompany(Guid companyId, [FromBody] EmployeeForCreationDto employee, CancellationToken ct)
    {
        var employeeToReturn = await _service.EmployeeService.CreateEmployeeForCompanyAsync(companyId, employee, trackChanges: false, ct);

        return CreatedAtRoute("GetEmployeeForCompany", new { companyId, id = employeeToReturn.Id },
            employeeToReturn);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteEmployeeForCompany(Guid companyId, Guid id, CancellationToken ct)
    {
        await _service.EmployeeService.DeleteEmployeeForCompanyAsync(companyId, id, trackChanges: false, ct);

        return NoContent();
    }

    [HttpPut("{id:guid}")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> UpdateEmployeeForCompany(Guid companyId, Guid id, [FromBody] EmployeeForUpdateDto employee, CancellationToken ct)
    { 
        await _service.EmployeeService.UpdateEmployeeForCompanyAsync(companyId, id, employee, compTrackChanges: false, empTrackChanges: true, ct);

        return NoContent();
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> PartiallyUpdateEmployeeForCompany(Guid companyId, Guid id, [FromBody] JsonPatchDocument<EmployeeForUpdateDto> patchDoc, CancellationToken ct)
    {
        if (patchDoc is null)
            return BadRequest("patchDoc object sent from client is null.");

        var result = await _service.EmployeeService.GetEmployeeForPatchAsync(companyId, id,
            compTrackChanges: false, empTrackChanges: true, ct);

        patchDoc.ApplyTo(result.employeeToPatch, ModelState);

        TryValidateModel(result.employeeToPatch);

        if (!ModelState.IsValid)
            return UnprocessableEntity(ModelState);

        await _service.EmployeeService.SaveChangesForPatchAsync(result.employeeToPatch, result.employeeEntity, ct);

        return NoContent();
    }
}
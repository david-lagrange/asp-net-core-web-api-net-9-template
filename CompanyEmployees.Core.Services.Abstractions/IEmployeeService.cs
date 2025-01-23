using CompanyEmployees.Core.Domain.Entities;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;

namespace CompanyEmployees.Core.Services.Abstractions;

public interface IEmployeeService
{
    Task<(IEnumerable<EmployeeDto> employees, MetaData metaData)> GetEmployeesAsync(Guid companyId, EmployeeParameters employeeParameters, bool trackChanges, CancellationToken ct = default);
    Task<EmployeeDto> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges, CancellationToken ct = default);
    Task<EmployeeDto> CreateEmployeeForCompanyAsync(Guid companyId, EmployeeForCreationDto employeeForCreation, bool trackChanges, CancellationToken ct = default);
    Task DeleteEmployeeForCompanyAsync(Guid companyId, Guid id, bool trackChanges, CancellationToken ct = default);
    Task UpdateEmployeeForCompanyAsync(Guid companyId, Guid id, EmployeeForUpdateDto employeeForUpdate, bool compTrackChanges, bool empTrackChanges, CancellationToken ct = default);
    Task<(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)> GetEmployeeForPatchAsync(Guid companyId, Guid id, bool compTrackChanges, bool empTrackChanges, CancellationToken ct = default);
    Task SaveChangesForPatchAsync(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity, CancellationToken ct = default);
}
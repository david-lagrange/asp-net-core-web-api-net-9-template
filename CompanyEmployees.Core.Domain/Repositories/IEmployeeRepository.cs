using CompanyEmployees.Core.Domain.Entities;
using Shared.RequestFeatures;

namespace CompanyEmployees.Core.Domain.Repositories;

public interface IEmployeeRepository
{
    Task<PagedList<Employee>> GetEmployeesAsync(Guid companyId, EmployeeParameters employeeParameters, bool trackChanges, CancellationToken ct = default);
    Task<Employee?> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges, CancellationToken ct = default);
    void CreateEmployeeForCompany(Guid companyId, Employee employee);
    Task DeleteEmployeeAsync(Company company, Employee employee, CancellationToken ct = default);
}

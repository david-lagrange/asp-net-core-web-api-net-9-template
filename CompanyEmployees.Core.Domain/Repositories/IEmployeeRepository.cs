using CompanyEmployees.Core.Domain.Entities;

namespace CompanyEmployees.Core.Domain.Repositories;

public interface IEmployeeRepository
{
    Task<IEnumerable<Employee>> GetEmployeesAsync(Guid companyId, bool trackChanges, CancellationToken ct = default);
    Task<Employee?> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges, CancellationToken ct = default);
    void CreateEmployeeForCompany(Guid companyId, Employee employee);
    Task DeleteEmployeeAsync(Company company, Employee employee, CancellationToken ct = default);
}

using CompanyEmployees.Core.Domain.Repositories;
using CompanyEmployees.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.RequestFeatures;

namespace CompanyEmployees.Infrastructure.Persistence.Repositories;

internal sealed class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
{
    public EmployeeRepository(RepositoryContext repositoryContext)
        : base(repositoryContext)
    {
    }

    public async Task<PagedList<Employee>> GetEmployeesAsync(Guid companyId, EmployeeParameters employeeParameters, bool trackChanges, CancellationToken ct = default)
    {
        var employees = await FindByCondition(e => e.CompanyId.Equals(companyId), trackChanges)
            .OrderBy(e => e.Name)
            .ToListAsync(ct);

        return PagedList<Employee>
            .ToPagedList(employees, employeeParameters.PageNumber, employeeParameters.PageSize);
    }

    public async Task<Employee?> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges, CancellationToken ct = default) =>
        await FindByCondition(e => e.CompanyId.Equals(companyId) && e.Id.Equals(id), trackChanges)
        .SingleOrDefaultAsync(ct);

    public void CreateEmployeeForCompany(Guid companyId, Employee employee)
    {
        employee.CompanyId = companyId;
        Create(employee);
    }

    public async Task DeleteEmployeeAsync(Company company, Employee employee, CancellationToken ct = default)
    {
        using var transaction = await RepositoryContext.Database.BeginTransactionAsync(ct);

        Delete(employee);

        await RepositoryContext.SaveChangesAsync(ct);

        if (!FindByCondition(e => e.CompanyId == company.Id, false).Any())
        {
            RepositoryContext.Companies!.Remove(company);

            await RepositoryContext.SaveChangesAsync(ct);
        }

        await transaction.CommitAsync(ct);
    }
}

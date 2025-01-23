using CompanyEmployees.Core.Domain.Repositories;
using CompanyEmployees.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.RequestFeatures;
using CompanyEmployees.Infrastructure.Persistence.Extensions;

namespace CompanyEmployees.Infrastructure.Persistence.Repositories;

internal sealed class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
{
    public EmployeeRepository(RepositoryContext repositoryContext)
        : base(repositoryContext)
    {
    }

    public async Task<PagedList<Employee>> GetEmployeesAsync(Guid companyId, EmployeeParameters employeeParameters, bool trackChanges, CancellationToken ct = default)
    {
        var employeesQuery = FindByCondition(e => e.CompanyId.Equals(companyId), trackChanges)
            .FilterEmployees(employeeParameters.MinAge, employeeParameters.MaxAge)
            .Search(employeeParameters.SearchTerm ?? string.Empty)
            .OrderBy(e => e.Name);

        var count = await employeesQuery.CountAsync(ct);

        var employees = await employeesQuery
            .Skip((employeeParameters.PageNumber - 1) * employeeParameters.PageSize)
            .Take(employeeParameters.PageSize)
            .ToListAsync(ct);

        return PagedList<Employee>
            .ToPagedList(employees, count, employeeParameters.PageNumber,
                employeeParameters.PageSize);
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

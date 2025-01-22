using Microsoft.EntityFrameworkCore.Storage;

namespace CompanyEmployees.Core.Domain.Repositories;

public interface IRepositoryManager
{
    ICompanyRepository Company { get; }
    IEmployeeRepository Employee { get; }
    Task SaveAsync(CancellationToken ct = default);
    IDbContextTransaction BeginTransaction();
}

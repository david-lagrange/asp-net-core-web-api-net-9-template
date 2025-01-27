using NetCoreWebAPIJWTAuth.Core.Domain.Entities;
using System.Reflection;
using System.Text;
using System.Linq.Dynamic.Core;
using NetCoreWebAPIJWTAuth.Infrastructure.Persistence.Extensions.Utility;

namespace NetCoreWebAPIJWTAuth.Infrastructure.Persistence.Extensions;

public static class RepositoryDependantEntityExtensions
{
    public static IQueryable<DependantEntity> FilterDependantEntitys(this IQueryable<DependantEntity> employees,
        uint minAge, uint maxAge) =>
        employees.Where(e => (e.Age >= minAge && e.Age <= maxAge));

    public static IQueryable<DependantEntity> Search(this IQueryable<DependantEntity> employees, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return employees;

        var lowerCaseTerm = searchTerm.Trim().ToLower();

        return employees.Where(e => e.Name != null && e.Name.Contains(lowerCaseTerm, StringComparison.OrdinalIgnoreCase));
    }

    public static IQueryable<DependantEntity> Sort(this IQueryable<DependantEntity> employees,
        string orderByQueryString)
    {
        if (string.IsNullOrWhiteSpace(orderByQueryString))
            return employees.OrderBy(e => e.Name);

        var orderQuery = OrderQueryBuilder.CreateOrderQuery<DependantEntity>(orderByQueryString);

        if (string.IsNullOrWhiteSpace(orderQuery))
            return employees.OrderBy(e => e.Name);

        return employees.OrderBy(orderQuery);
    }
}

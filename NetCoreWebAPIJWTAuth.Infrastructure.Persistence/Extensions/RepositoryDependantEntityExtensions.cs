using NetCoreWebAPIJWTAuth.Core.Domain.Entities;
using System.Linq.Dynamic.Core;
using NetCoreWebAPIJWTAuth.Infrastructure.Persistence.Extensions.Utility;

namespace NetCoreWebAPIJWTAuth.Infrastructure.Persistence.Extensions;

public static class RepositoryDependantEntityExtensions
{
    public static IQueryable<DependantEntity> Search(this IQueryable<DependantEntity> dependantEntities, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return dependantEntities;

        var lowerCaseTerm = searchTerm.Trim().ToLower();

        return dependantEntities.Where(e => e.Name != null && e.Name.Contains(lowerCaseTerm, StringComparison.OrdinalIgnoreCase));
    }

    public static IQueryable<DependantEntity> Sort(this IQueryable<DependantEntity> dependantEntities, string orderByQueryString)
    {
        if (string.IsNullOrWhiteSpace(orderByQueryString))
            return dependantEntities.OrderBy(e => e.Name);

        var orderQuery = OrderQueryBuilder.CreateOrderQuery<DependantEntity>(orderByQueryString);

        if (string.IsNullOrWhiteSpace(orderQuery))
            return dependantEntities.OrderBy(e => e.Name);

        return dependantEntities.OrderBy(orderQuery);
    }
}

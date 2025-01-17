using CompanyEmployees.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CompanyEmployees.Infrastructure.Persistence
{
    public class RepositoryContext : DbContext
    {
        public RepositoryContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Company>? Companies { get; set; }
        public DbSet<Employee>? Employees { get; set; }
    }
}

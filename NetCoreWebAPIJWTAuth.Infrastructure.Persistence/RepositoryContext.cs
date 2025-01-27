using NetCoreWebAPIJWTAuth.Core.Domain.Entities;
using NetCoreWebAPIJWTAuth.Infrastructure.Persistence.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace NetCoreWebAPIJWTAuth.Infrastructure.Persistence;

public class RepositoryContext : IdentityDbContext<User>
{
    public RepositoryContext(DbContextOptions<RepositoryContext> options)
        : base(options)
    {
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new BaseEntityConfiguration());
        modelBuilder.ApplyConfiguration(new DependantEntityConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
    }

    public DbSet<BaseEntity>? Companies { get; set; }
    public DbSet<DependantEntity>? DependantEntitys { get; set; }
}
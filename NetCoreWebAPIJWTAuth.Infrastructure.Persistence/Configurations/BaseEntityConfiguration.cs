using NetCoreWebAPIJWTAuth.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace NetCoreWebAPIJWTAuth.Infrastructure.Persistence.Configurations;

public class BaseEntityConfiguration : IEntityTypeConfiguration<BaseEntity>
{
    public void Configure(EntityTypeBuilder<BaseEntity> builder)
    {
        builder.ToTable("Companies");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
               .HasColumnName("BaseEntityId")
               .IsRequired();
        builder.Property(c => c.Name)
               .IsRequired()
               .HasMaxLength(60);
        builder.Property(c => c.Address)
               .IsRequired()
               .HasMaxLength(60);
        builder.Property(c => c.Country);
        builder.HasMany(c => c.DependantEntitys)
               .WithOne(e => e.BaseEntity)
               .HasForeignKey(e => e.BaseEntityId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasData
        (
            new BaseEntity
            {
                Id = new Guid("c9d4c053-49b6-410c-bc78-2d54a9991870"),
                Name = "IT_Solutions Ltd",
                Address = "583 Wall Dr. Gwynn Oak, MD 21207",
                Country = "USA"
            },
            new BaseEntity
            {
                Id = new Guid("3d490a70-94ce-4d15-9494-5248280c2ce3"),
                Name = "Admin_Solutions Ltd",
                Address = "312 Forest Avenue, BF 923",
                Country = "USA"
            }
        );
    }
}

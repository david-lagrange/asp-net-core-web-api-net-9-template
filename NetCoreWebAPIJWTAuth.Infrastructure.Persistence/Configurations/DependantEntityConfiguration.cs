using NetCoreWebAPIJWTAuth.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace NetCoreWebAPIJWTAuth.Infrastructure.Persistence.Configurations;

public class DependantEntityConfiguration : IEntityTypeConfiguration<DependantEntity>
{
    public void Configure(EntityTypeBuilder<DependantEntity> builder)
    {
        builder.ToTable("DependantEntities");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
               .HasColumnName("DependantEntityId")
               .IsRequired();
        builder.Property(e => e.Name)
               .IsRequired()
               .HasMaxLength(30);
        builder.Property(e => e.Age)
               .IsRequired();
        builder.Property(e => e.Position)
               .IsRequired()
               .HasMaxLength(20);
        builder.HasOne(e => e.BaseEntity)
               .WithMany(c => c.DependantEntities)
               .HasForeignKey(e => e.BaseEntityId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasData
        (
            new DependantEntity
            {
                Id = new Guid("80abbca8-664d-4b20-b5de-024705497d4a"),
                Name = "Sam Raiden",
                Age = 26,
                Position = "Software developer",
                BaseEntityId = new Guid("c9d4c053-49b6-410c-bc78-2d54a9991870")
            },
            new DependantEntity
            {
                Id = new Guid("86dba8c0-d178-41e7-938c-ed49778fb52a"),
                Name = "Jana McLeaf",
                Age = 30,
                Position = "Software developer",
                BaseEntityId = new Guid("c9d4c053-49b6-410c-bc78-2d54a9991870")
            },
            new DependantEntity
            {
                Id = new Guid("021ca3c1-0deb-4afd-ae94-2159a8479811"),
                Name = "Kane Miller",
                Age = 35,
                Position = "Administrator",
                BaseEntityId = new Guid("3d490a70-94ce-4d15-9494-5248280c2ce3")
            }
        );
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using File = ApiTemplate.Domain.Entities.File;

namespace ApiTemplate.Persistence.Configurations;

public class FileConfiguration : EntityTypeConfigurationDependency<File>
{
    public override void Configure(EntityTypeBuilder<File> builder)
    {
        builder
            .ToTable("File")
            .HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever();

        builder.Property(e => e.Src)
            .HasMaxLength(400);

        builder.Property(e => e.Name)
            .HasMaxLength(150);

        builder.Property(e => e.Mime)
            .HasMaxLength(100);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ApiTemplate.Domain.Entities.Customers;

namespace ApiTemplate.Persistence.Configurations.Customers;

public class CouponConfiguration : EntityTypeConfigurationDependency<Coupon>
{
    public override void Configure(EntityTypeBuilder<Coupon> builder)
    {
        builder
            .ToTable("Coupon")
            .HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever();

        builder.Property(e => e.Code)
            .HasMaxLength(10);

        builder.Property(e => e.Description)
            .HasMaxLength(100);

        builder.Property(e => e.DiscountPercent)
            .HasPrecision(5, 2);

        builder.HasOne(e => e.CreatedByUser)
            .WithMany(e => e.CreatedCoupons)
            .HasForeignKey(e => e.CreatedByUserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasIndex(e => e.Code)
            .IsUnique();
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ApiTemplate.Domain.Entities.Customers;

namespace ApiTemplate.Persistence.Configurations.Customers;

public class CouponCompanyUserConfiguration : EntityTypeConfigurationDependency<CouponCompanyUser>
{
    public override void Configure(EntityTypeBuilder<CouponCompanyUser> builder)
    {
        builder
            .ToTable("CouponCompanyUser")
            .HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever();

        builder.HasOne(e => e.Coupon)
            .WithMany(e => e.CouponCompanyUsers)
            .HasForeignKey(e => e.CouponId)
            .OnDelete(DeleteBehavior.ClientCascade);

        builder.HasOne(e => e.CompanyUser)
            .WithMany(e => e.CouponCompanyUsers)
            .HasForeignKey(e => e.CompanyUserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}

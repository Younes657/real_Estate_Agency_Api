using AgenceImmobiliareApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgenceImmobiliareApi.Data.Configuration
{
    public class RealEstateConfig : IEntityTypeConfiguration<RealEstate>
    {
        public void Configure(EntityTypeBuilder<RealEstate> builder)
        {
            builder.Property(x => x.Id).HasDefaultValue(100);

            builder.HasOne(x => x.Category).WithMany(x => x.RealEstates).HasForeignKey(x => x.CategoryId).IsRequired();

            builder.HasOne(x => x.Addresse).WithOne(x => x.RealEstate).HasForeignKey<RealEstate>(x => x.AddressId).IsRequired();

        }
    }
}

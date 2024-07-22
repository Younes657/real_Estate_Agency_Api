using AgenceImmobiliareApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgenceImmobiliareApi.Data.Configuration
{
    public class RealEstateConfig : IEntityTypeConfiguration<RealEstate>
    {
        public void Configure(EntityTypeBuilder<RealEstate> builder)
        {
           
            builder.HasOne(x => x.Category).WithMany(x => x.RealEstates).HasForeignKey(x => x.CategoryId).IsRequired();

            builder.HasOne(x => x.Addresse).WithOne(x => x.RealEstate).HasForeignKey<RealEstate>(x => x.AddressId).IsRequired();

            builder.Property(e => e.Price)
             .HasColumnType("decimal(18, 2)") // Adjust precision and scale as needed
             .HasPrecision(18, 2); // Explicitly set precision and scale

            builder.Property(e => e.Surface)
                  .HasColumnType("decimal(18, 2)") // Adjust precision and scale as needed
                  .HasPrecision(18, 2); // Explicitly set precision and scale

        }
    }
}

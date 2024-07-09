using AgenceImmobiliareApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgenceImmobiliareApi.Data.Configuration
{
    public class ImageConfig: IEntityTypeConfiguration<Image>
    {
        public void Configure(EntityTypeBuilder<Image> builder)
        {
            builder.HasOne(x => x.RealEstate).WithMany(x => x.Images).HasForeignKey(x => x.RealEstateId).IsRequired();
        }

    }
}

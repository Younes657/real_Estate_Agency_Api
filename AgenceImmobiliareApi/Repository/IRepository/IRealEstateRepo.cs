using AgenceImmobiliareApi.Models;

namespace AgenceImmobiliareApi.Repository.IRepository
{
    public interface IRealEstateRepo : IRepository<RealEstate>
    {
        void Update(RealEstate realEstate);
    }
}

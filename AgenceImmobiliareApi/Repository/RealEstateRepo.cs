using AgenceImmobiliareApi.Data;
using AgenceImmobiliareApi.Models;
using AgenceImmobiliareApi.Repository.IRepository;

namespace AgenceImmobiliareApi.Repository
{
    public class RealEstateRepo : Repository<RealEstate>, IRealEstateRepo
    {
        private readonly AppDbContext _db;
        public RealEstateRepo(AppDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(RealEstate realEstate)
        {
            _db.RealEstates.Update(realEstate);
        }
    }
}


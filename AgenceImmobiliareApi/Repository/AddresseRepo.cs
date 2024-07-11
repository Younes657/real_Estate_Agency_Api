using AgenceImmobiliareApi.Data;
using AgenceImmobiliareApi.Models;
using AgenceImmobiliareApi.Repository.IRepository;

namespace AgenceImmobiliareApi.Repository
{
    public class AddresseRepo : Repository<Addresse>, IAddresseRepo
    {
        private readonly AppDbContext _db;
        public AddresseRepo(AppDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Addresse addresse)
        {
            _db.Addresses.Update(addresse);
        }
    }
}

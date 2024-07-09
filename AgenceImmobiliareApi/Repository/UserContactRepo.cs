using AgenceImmobiliareApi.Data;
using AgenceImmobiliareApi.Models;
using AgenceImmobiliareApi.Repository.IRepository;

namespace AgenceImmobiliareApi.Repository
{
    public class UserContactRepo : Repository<UserContact>, IUserContactRepo
    {
        private readonly AppDbContext _db;
        public UserContactRepo(AppDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(UserContact userContact)
        {
            _db.UserContacts.Update(userContact);
        }
    }
}

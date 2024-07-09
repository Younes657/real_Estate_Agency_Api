using AgenceImmobiliareApi.Data;
using AgenceImmobiliareApi.Models;
using AgenceImmobiliareApi.Repository.IRepository;

namespace AgenceImmobiliareApi.Repository
{
    public class CategroyRepo : Repository<Category>, ICategoryRepo
    {
        private readonly AppDbContext _db;
        public CategroyRepo(AppDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Category category)
        {
            _db.Categories.Update(category);
        }
    }
}

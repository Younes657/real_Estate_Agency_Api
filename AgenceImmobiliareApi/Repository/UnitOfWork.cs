using AgenceImmobiliareApi.Data;
using AgenceImmobiliareApi.Repository.IRepository;

namespace AgenceImmobiliareApi.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private AppDbContext _db;
        public ICategoryRepo CategoryRepo { get; private set; }
        public IBlogArticleRepo BlogArticleRepo { get; private set; }
        public IRealEstateRepo RealEstateRepo { get; private set; }
        public IImageRepo ImageRepo { get; private set; }
        public IUserContactRepo UserContactRepo { get; private set; }

        public UnitOfWork(AppDbContext db)
        {
            _db = db;
            CategoryRepo = new CategroyRepo(_db);
            RealEstateRepo = new RealEstateRepo(_db);
            ImageRepo = new ImageRepo(_db);
            UserContactRepo = new UserContactRepo(_db);
            BlogArticleRepo = new BlogArticleRepo(_db);
            
        }
        public AppDbContext AppDbContext()
        {
            return _db;
        }

        public void Save()
        {
            _db.SaveChanges();
        }

    }
}

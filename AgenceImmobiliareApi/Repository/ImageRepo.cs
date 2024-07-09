using AgenceImmobiliareApi.Data;
using AgenceImmobiliareApi.Models;
using AgenceImmobiliareApi.Repository.IRepository;

namespace AgenceImmobiliareApi.Repository
{
    public class ImageRepo : Repository<Image>, IImageRepo
    {
        private readonly AppDbContext _db;
        public ImageRepo(AppDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Image image)
        {
            _db.Images.Update(image);
        }
    }
}


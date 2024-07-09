using AgenceImmobiliareApi.Data;
using AgenceImmobiliareApi.Models;
using AgenceImmobiliareApi.Repository.IRepository;

namespace AgenceImmobiliareApi.Repository
{
    public class BlogArticleRepo : Repository<BlogArticle>, IBlogArticleRepo
    {
        private readonly AppDbContext _db;
        public BlogArticleRepo(AppDbContext db):base(db)
        {
            _db = db;   
        }

        public void Update(BlogArticle blogArticle)
        {
            _db.BlogArticles.Update(blogArticle);
        }
    }
}

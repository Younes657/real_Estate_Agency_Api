using AgenceImmobiliareApi.Models;

namespace AgenceImmobiliareApi.Repository.IRepository
{
    public interface IBlogArticleRepo : IRepository<BlogArticle>
    {
        void Update(BlogArticle blogArticle);
    }
}

using AgenceImmobiliareApi.Models;

namespace AgenceImmobiliareApi.Repository.IRepository
{
    public interface ICategoryRepo : IRepository<Category>
    {
        void Update(Category category);
    }
}

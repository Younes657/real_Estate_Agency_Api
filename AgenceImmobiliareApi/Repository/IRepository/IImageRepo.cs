using AgenceImmobiliareApi.Models;

namespace AgenceImmobiliareApi.Repository.IRepository
{
    public interface IImageRepo : IRepository<Image>
    {
        void Update(Image image);
    }
}
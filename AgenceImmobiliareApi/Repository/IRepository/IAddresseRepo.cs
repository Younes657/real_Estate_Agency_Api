using AgenceImmobiliareApi.Models;

namespace AgenceImmobiliareApi.Repository.IRepository
{
    public interface IAddresseRepo : IRepository<Addresse>
    {
        void Update(Addresse addresse);
    }
}

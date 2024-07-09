using AgenceImmobiliareApi.Models;

namespace AgenceImmobiliareApi.Repository.IRepository
{
    public interface IUserContactRepo : IRepository<UserContact>
    {
        void Update(UserContact userContact);
    }
}

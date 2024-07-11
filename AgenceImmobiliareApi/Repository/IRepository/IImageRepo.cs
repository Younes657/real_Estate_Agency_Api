using AgenceImmobiliareApi.Models;

namespace AgenceImmobiliareApi.Repository.IRepository
{
    public interface IImageRepo : IRepository<Image>
    {
        void Update(Image image);
        List<Image> UpsertImagesToFolder(IWebHostEnvironment webHostEnvironment, int RealEstateId , List<IFormFile>? files = null , bool? Deleted = null );
        Task AddRange(List<Image> images);
        void UpdateRange(List<Image> images);
    }
}
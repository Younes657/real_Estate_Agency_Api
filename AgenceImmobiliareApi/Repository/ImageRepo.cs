using AgenceImmobiliareApi.Data;
using AgenceImmobiliareApi.Models;
using AgenceImmobiliareApi.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

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
        public bool DeleteImage(IWebHostEnvironment webHostEnvironment, int RealEstateId , string link)
        {
            string wwwRootPath = webHostEnvironment.WebRootPath;
            string fileName = link.Split("\\").Last();

            if (!string.IsNullOrEmpty(fileName)) 
            { 
                string imagePath = Path.Combine(wwwRootPath, @$"Images\RealEstates\RE-{RealEstateId}\{fileName}");
                if (System.IO.File.Exists(imagePath))
                {
                    try
                    {
                        System.IO.File.Delete(imagePath);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
        public List<Image> UpsertImagesToFolder(IWebHostEnvironment webHostEnvironment, int RealEstateid, List<IFormFile>? files = null , bool? Deleted=null )
        {
            string wwwRootPath = webHostEnvironment.WebRootPath;
            
            if (Deleted == true )
            {
                string DirectoryPath = Path.Combine(wwwRootPath, @"Images\RealEstates\RE-" + RealEstateid);
                if(Directory.Exists(DirectoryPath))
                    Directory.Delete(DirectoryPath, true);
            }
            List<Image> Images = [];
            if (files != null)
            {
                foreach (IFormFile file in files)
                {
                    string filename = Guid.NewGuid().ToString() + Path.GetFileName(file.FileName);

                    var FinalPath = Path.Combine(wwwRootPath, @"Images\RealEstates\RE-" + RealEstateid);

                    if (!Directory.Exists(FinalPath))
                        Directory.CreateDirectory(FinalPath);
                    using (FileStream stream = new FileStream(Path.Combine(FinalPath, filename), FileMode.Create))
                    {
                        //Copies the contents of the uploaded file to the target stream.
                        file.CopyTo(stream);
                    }
                    Images.Add(new Image
                    {
                        ImageLink = $@"https://localhost:7078\Images\RealEstates\RE-{RealEstateid}\{filename}",
                        RealEstateId = RealEstateid

                    });
                }
            }
            return Images;
        }
        public async Task AddRange(List<Image> images)
        {
            await _db.Images.AddRangeAsync(images);
        }
        public void UpdateRange(List<Image> images)
        {
             _db.Images.UpdateRange();
        }
    }
}


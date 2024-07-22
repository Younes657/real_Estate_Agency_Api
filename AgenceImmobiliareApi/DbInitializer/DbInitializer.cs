using AgenceImmobiliareApi.Data;
using AgenceImmobiliareApi.Models;
using AgenceImmobiliareApi.Repository;
using AgenceImmobiliareApi.Repository.IRepository;
using AgenceImmobiliareApi.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AgenceImmobiliareApi.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<ApplicationUser> _UserManager;
        private readonly RoleManager<IdentityRole> _RoleManager;
        private readonly IUnitOfWork _UnitOfWork;
        public DbInitializer(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> userRole,
            IUnitOfWork unitOfWork)
        {
            _UserManager = userManager;
            _RoleManager = userRole;
            _UnitOfWork = unitOfWork;
        }
        public void Initialize()
        {
            //we should create migration if thery are not applied
            try
            {
                if (_UnitOfWork.AppDbContext().Database.GetPendingMigrations().Count() > 0)
                {
                    _UnitOfWork.AppDbContext().Database.Migrate();
                }
            }
            catch (Exception ex) 
            { }
            // we create the roles
            if (!_RoleManager.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult())
            {
                _RoleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                _RoleManager.CreateAsync(new IdentityRole(SD.Role_User)).GetAwaiter().GetResult();

                //then we create the first user admin
               IdentityResult? result = _UserManager.CreateAsync(new ApplicationUser()
                {
                    UserName = "Nari",
                    Email = "agence_immobiliere_Nari@immo-nari.com",
                    PhoneNumber = "0550545668"
                }, "123Nari@").GetAwaiter().GetResult();

                if (result != null && result.Succeeded)
                {
                    //now let's get the user 
                    ApplicationUser user = _UnitOfWork.AppDbContext().ApplicationUsers.FirstOrDefault(x => x.Email == "agence_immobiliere_Nari@immo-nari.com");
                    _UserManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();
                }
               
            }
            return;
        }
    }
}

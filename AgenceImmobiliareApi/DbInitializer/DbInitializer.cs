using AgenceImmobiliareApi.Data;
using AgenceImmobiliareApi.Models;
using AgenceImmobiliareApi.Repository;
using AgenceImmobiliareApi.Repository.IRepository;
using AgenceImmobiliareApi.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

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
            if (_UnitOfWork.AppDbContext().WebSiteInfos.ToList().Count() <= 0) 
            {
                WebSiteInfo info = new WebSiteInfo()
                {
                    Wilaya = "Alger",
                    Ville = "Birkhadem",
                    Rue = "Lots les rosiers",
                    streetNumber = "23",
                    PostalCode = "16330",
                    Email= "info@immo-nari.com",
                    PhoneNumber= "0550 54 56 68",
                    WhatUpNumber= "0550 54 56 68",
                    linkdinLink = "https://www.linkedin.com/in/saddek-gozim-1339322b/?originalSubdomain=dz",
                    FacebookLink = "https://web.facebook.com/Narimmo?_rdc=1&_rdr"
                };
                _UnitOfWork.AppDbContext().WebSiteInfos.AddAsync(info).GetAwaiter().GetResult();
                _UnitOfWork.Save().GetAwaiter().GetResult();
            }
            return;
        }
    }
}

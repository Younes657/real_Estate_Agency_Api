using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using AgenceImmobiliareApi.Repository.IRepository;
using AgenceImmobiliareApi.Repository;
using AgenceImmobiliareApi.DbInitializer;

namespace AgenceImmobiliareApi.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()) // Set the base path to the current directory
            .AddJsonFile("appsettings.json") // Add appsettings.json file
            .Build(); // Build the configuration


            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            
            var connectionString = configuration.GetConnectionString("defaultctr");
            optionsBuilder.UseSqlServer(connectionString);

            //var services = new ServiceCollection();
            //services.AddScoped<IUnitOfWork, UnitOfWork>();
            //using (var serviceProvider = services.BuildServiceProvider())
            //{
            //    var dbInitializer = serviceProvider.GetRequiredService<IDbInitializer>();
            //    // Perform any necessary initialization here
            //}

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}

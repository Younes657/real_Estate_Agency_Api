using AgenceImmobiliareApi.Data;
using AgenceImmobiliareApi.Models;
using AgenceImmobiliareApi.Repository.IRepository;
using AgenceImmobiliareApi.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AgenceImmobiliareApi.DbInitializer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// add the dbcontext 
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("defaultctr"));
});

builder.Services.AddIdentity<ApplicationUser , IdentityRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
//builder.Services.Configure<IdentityOptions>(options =>
//{
//    options.Password.RequireDigit = false;
//    options.Password.RequiredLength = 1;
//    options.Password.RequireLowercase = false;
//    options.Password.RequireUppercase = false;
//    options.Password.RequireNonAlphanumeric = false;
//});

var key = builder.Configuration.GetValue<string>("ApiSettings:SecretKey");
builder.Services.AddAuthentication(u =>
{
    u.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; //const Bearer
    u.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(u =>
{
    u.RequireHttpsMetadata = false;
    u.SaveToken = true;
    u.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false, // if we have a certain url can sent the token we should define here
    };
});

builder.Services.AddCors(
//    options =>
//{
//    options.AddPolicy("AllowSpecificOrigins",
//        builder =>
//        {
//            builder.WithOrigins("https://example.com", "https://another-example.com")
//                   .AllowAnyHeader()
//                   .AllowAnyMethod();
//        });
//}
); //so if the api is called from some other urls it will work

//ignore circular references during serialization (two objects reference each other)
builder.Services.AddControllers().AddJsonOptions(x =>
   x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

//add the automapper to services
builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IDbInitializer, DbInitializer>();


builder.Services.AddControllers();



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//add the automapper to services



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCors(); //app.UseCors("AllowSpecificOrigins");
//[EnableCors("AllowSpecificOrigins")] in controllers

app.UseAuthentication();
app.UseAuthorization();

SeedDb();

app.MapControllers();

app.Run();


void SeedDb()
{
    using (var scope = app.Services.CreateScope())
    {
        IDbInitializer dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        dbInitializer.Initialize();
    }
}
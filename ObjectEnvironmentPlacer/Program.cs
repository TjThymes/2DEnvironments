using ObjectEnvironmentPlacer.Data;
using ObjectEnvironmentPlacer.Objects;
using ObjectEnvironmentPlacer.Other;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text;
using ObjectEnvironmentPlacer.Configuration;
using ObjectEnvironmentPlacer.Data;
using ObjectEnvironmentPlacer.Interface;
using ObjectEnvironmentPlacer.Objects;
using ObjectEnvironmentPlacer.Other;
using System.Runtime.InteropServices.Marshalling;
using ObjectEnvironmentPlacer.Repositories;

var builder = WebApplication.CreateBuilder(args);

#region 🔧 Configuration & Logging

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

builder.Logging.AddConsole();

#endregion

#region 🔌 Services & Repositories

builder.Services.AddControllers();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEnvironment2DRepository>(provider => new Environment2DRepository(
    builder.Configuration.GetConnectionString("SqlConnectionString")));
builder.Services.AddScoped<IObjectRepository>(provider => new ObjectRepository(
    builder.Configuration.GetConnectionString("SqlConnectionString")));
builder.Services.AddScoped<IPlayerEnvironmentRepository>(provider => new PlayerEnvironmentRepository(
    builder.Configuration.GetConnectionString("SqlConnectionString")));

builder.Services.AddScoped<JwtTokenGenerator>();
builder.Services.AddJwtAuthentication(builder.Configuration); // ✅ ONLY THIS for JWT setup

#endregion

#region 💾 Database & Identity

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnectionString")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

#endregion

#region 🧪 Swagger (With JWT Support)

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "2DWorld API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer {token}')",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

#endregion

#region 🚀 App Pipeline

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "2DWorld API v1");
    c.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

#endregion

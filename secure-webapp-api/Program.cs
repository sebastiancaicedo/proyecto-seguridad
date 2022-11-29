using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore.InMemory;
using secure_webapp_api;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

internal class Program
{
  private static void Main(string[] args)
  {
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
      options.SwaggerDoc("v1", new OpenApiInfo
      {
        Version = "v1",
        Title = "Secure App",
        Description = "Web API Restful para la secure app",
        Contact = new OpenApiContact
        {
          Name = "Luis Caicedo",
          // Url = new Uri("https://example.com/contact")
          Email = "sebastiancaicedo@uninorte.edu.co"
        },
        // License = new OpenApiLicense
        // {
        //   Name = "Example License",
        //   Url = new Uri("https://example.com/license")
        // }
      });
    });

    builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("db")));
    // builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("db"));

    var key = Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("JwtSecretKey"));

    builder.Services.AddAuthentication(x =>
    {
      x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
      x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

    }).AddJwtBearer(x =>
    {
      x.RequireHttpsMetadata = false;
      x.SaveToken = true;
      x.TokenValidationParameters = new TokenValidationParameters
      {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
      };
    });


    var app = builder.Build();

    // Configure the HTTP request pipeline.
    app.UseDeveloperExceptionPage();


    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
      options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
      options.RoutePrefix = string.Empty;
    });


    app.UseHttpsRedirection();

    app.UseMiddleware<ExceptionsMiddleware>();

    app.UseAuthentication();

    app.UseRouting();

    app.UseAuthorization();

    app.UseCors(options =>
    {
      string origin = builder.Configuration.GetValue<string>("WebAppOriginUrl");
      options.WithOrigins(origin).WithHeaders("content-type", "authorization").WithMethods("GET", "POST");
    });

    app.MapControllers();

    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetService<AppDbContext>();
    context!.Database.EnsureCreated();

    app.Run();
  }
}

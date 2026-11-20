using ContactsManager.Core.Domain.IdentityEntities;
using CRUDExample.Filters.ActionFilters;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repositories;
using RepositoryContracts;
using ServiceContracts;
using Services;

namespace CRUDExample
{
  public static class ConfigureServicesExtension
  {
    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
      services.AddDbContext<ApplicationDbContext>(options =>
      {
        options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
      });

      services.AddTransient<PersonsListActionFilter>();

      // Enable Identity with Guid-based User and Role
      services
          .AddIdentity<ApplicationUser, ApplicationRole>(options =>
          {
            options.Password.RequiredLength = 5;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = true;
            options.Password.RequireDigit = false;
            options.Password.RequiredUniqueChars = 3;
          })
          .AddEntityFrameworkStores<ApplicationDbContext>()
          .AddDefaultTokenProviders();

      //Authorization Policy
      services.AddAuthorization(options =>
      {
        options.FallbackPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();  //Enforce authorization policy (user must be authenticated) for all the action methods


        options.AddPolicy("NotAuthorized", policy =>
        {
          policy.RequireAssertion(context =>
          {
            return !context.User.Identity.IsAuthenticated;
          });
        });
      });

      services.ConfigureApplicationCookie(option =>
      {
        option.LoginPath = "/Account/Login";
      });

      // Register HTTP logging services
      services.AddHttpLogging(options =>
      {
        options.LoggingFields =
            Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestPropertiesAndHeaders |
            Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponsePropertiesAndHeaders;
      });

      services.AddTransient<ResponseHeaderActionFilter>();

      //It adds controllers and views as services
      services.AddControllersWithViews(options =>
      {
        //options.Filters.Add<ResponseHeaderActionFilter>();

        var logger = services.BuildServiceProvider().GetRequiredService<ILogger<ResponseHeaderActionFilter>>();

        options.Filters.Add(new ResponseHeaderActionFilter(logger)
        {
          Key = "My-Key-From-Global",
          Value = "My-Value-From-Global",
          Order = 2
        });

        options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
      });

      //add services into IoC container
      services.AddScoped<ICountriesService, CountriesService>();
      services.AddScoped<IPersonsAdderService, PersonsAdderService>();
      services.AddScoped<IPersonsDeleterService, PersonsDeleterService>();
      services.AddScoped<IPersonsGetterService, PersonsGetterService>();
      services.AddScoped<IPersonsSorterService, PersonsSorterService>();
      services.AddScoped<IPersonsUpdaterService, PersonsUpdaterService>();
      services.AddScoped<ICountriesRepository, CountriesRepositoy>();
      services.AddScoped<IPersonsRepository, PersonsRepository>();

      return services;
    }
  }
}

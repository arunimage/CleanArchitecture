using CRUDExample;
using CRUDExample.Middleware;
using Microsoft.Extensions.FileSystemGlobbing.Internal.Patterns;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureServices(builder.Configuration);

//Serilog
builder.Host.UseSerilog((context, services, loggerConfiguration) =>
{
  loggerConfiguration
      .ReadFrom.Configuration(context.Configuration)      //read configuration settings from built-in Iconfiguration
      .ReadFrom.Services(services);           //Read out current app's service and make them available to serilog
});

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
  app.UseDeveloperExceptionPage();
}
else
{
  app.UseExceptionHandler("/Error");
  app.UseExceptionHandlingMiddleware();
}

app.UseHsts();
app.UseHttpsRedirection();

app.UseSerilogRequestLogging();

app.UseHttpLogging();

//app.Logger.LogDebug("debug-message");
//app.Logger.LogInformation("information-message");
//app.Logger.LogWarning("warning-message");
//app.Logger.LogError("error-message");
//app.Logger.LogCritical("critical-message");

Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");

app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.UseEndpoints(endpoints =>
{
  endpoints.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}");
  //Admin/Home/Index
  //Admin

  endpoints.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action}/{id?}"
    );
});

app.Run();

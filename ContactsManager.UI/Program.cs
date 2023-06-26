using CRUDExample;
using CRUDExample.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

////Logging ASP net core built in
//builder.Host.ConfigureLogging(loggingProvider => {
//    loggingProvider.ClearProviders();
//    loggingProvider.AddConsole();
//    loggingProvider.AddDebug();
//    loggingProvider.AddEventLog();
//});

//Serilog
builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) =>
{
    loggerConfiguration
    .ReadFrom.Configuration(context.Configuration) //read configuration settings from built-in IConfiguration
    .ReadFrom.Services(services); //read out current app's services and make them available to serilog
});

builder.Services.ConfigureServices(builder.Configuration);

var app = builder.Build();

//Create application pipeline
if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    //This is created for any kind of error in runtime
    app.UseExceptionHandler("/Error");
    app.UseExceptionHandlingMiddleware();
}

//Add the diagnostics serilog
app.UseSerilogRequestLogging();



//Enable http logging
app.UseHttpLogging();

//Create logging
//app.Logger.LogDebug("debug-message");
//app.Logger.LogInformation("information-message");
//app.Logger.LogWarning("warning-message");
//app.Logger.LogError("error-message");
//app.Logger.LogCritical("critical-message");

//We will skip this for the integration testing
if (builder.Environment.IsEnvironment("Test") == false)
    Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");

app.UseStaticFiles();

app.UseRouting(); // Identifying action method based on the route
app.UseAuthentication(); // This is to read the cookies
app.UseAuthorization(); //Evaluates if the particular user has access or not
app.MapControllers(); //Is responsible for execution of filter pipeline (actions + filters)

app.UseEndpoints(endpoints => {
    endpoints.MapControllerRoute(
     name: "areas",
     pattern: "{area:exists}/{controller=Home}/{action=Index}");

    //Admin/Home/Index
    //Admin
    endpoints.MapControllerRoute(
     name: "default",
     pattern: "{controller}/{action}");
 });

//Eg: /persons/edit/1

app.Run();

//This piece of code makes the program a partial class // This will only be accessible in the test project
public partial class Program
{ }
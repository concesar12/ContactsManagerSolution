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
app.UseRouting();
app.MapControllers();
app.Run();

//This piece of code makes the program a partial class // This will only be accessible in the test project
public partial class Program
{ }
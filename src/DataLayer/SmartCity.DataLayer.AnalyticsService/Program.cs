using CoreWCF;
using CoreWCF.Channels;
using CoreWCF.Configuration;
using CoreWCF.Description;
using SmartCity.DataLayer.AnalyticsService.Contracts;
using SmartCity.DataLayer.AnalyticsService.Infrastructure;
using SmartCity.DataLayer.AnalyticsService.Repositories;
using SmartCity.DataLayer.AnalyticsService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:5106");

var tripsDbConnectionString = builder.Configuration.GetConnectionString("AnalyticsDatabase") ?? "Host=localhost;Port=5432;Database=smartcity_trips;Username=myuser;Password=mypassword";
var vehiclesDbConnectionString = builder.Configuration.GetConnectionString("UserDatabase") ?? "Host=localhost;Port=5432;Database=smartcity_vehicles;Username=myuser;Password=mypassword";
var usersDbConnectionString = builder.Configuration.GetConnectionString("UserDatabase") ?? "Host=localhost;Port=5432;Database=smartcity_users;Username=myuser;Password=mypassword";

builder.Services.AddSingleton(provider => new DatabaseConnectionFactory(tripsDbConnectionString));
builder.Services.AddScoped<AnalyticsRepository>();
builder.Services.AddScoped<AnalyticsDataService>();

builder.Services.AddServiceModelServices();
builder.Services.AddServiceModelMetadata();

var app = builder.Build();

app.UseServiceModel(serviceBuilder =>
{
    serviceBuilder.AddService<AnalyticsDataService>(options =>
    {
        options.DebugBehavior.IncludeExceptionDetailInFaults = true;
    });

    serviceBuilder.AddServiceEndpoint<AnalyticsDataService, IAnalyticsDataService>(
        new BasicHttpBinding(BasicHttpSecurityMode.None),
        "/AnalyticsDataService.svc"
    );

    var serviceMetadataBehavior = app.Services.GetRequiredService<ServiceMetadataBehavior>();
    serviceMetadataBehavior.HttpGetEnabled = true;
});

app.MapGet("/", () => Results.Redirect("/AnalyticsDataService.svc"));
app.MapGet("/health", () => Results.Ok(new { 
    status = "healthy", 
    service = "AnalyticsDataService",
    timestamp = DateTime.UtcNow 
}));

Console.WriteLine("AnalyticsDataService starting on http://localhost:5106");
app.Run();
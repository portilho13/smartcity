using CoreWCF;
using CoreWCF.Channels;
using CoreWCF.Configuration;
using CoreWCF.Description;
using SmartCity.DataLayer.TripService.Contracts;
using SmartCity.DataLayer.TripService.Infrastructure;
using SmartCity.DataLayer.TripService.Repositories;
using SmartCity.DataLayer.TripService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:5103");


var tripDbConnectionString = builder.Configuration.GetConnectionString("TripDatabase")  ?? "Host=localhost;Port=5432;Database=smartcity_trips;Username=myuser;Password=mypassword";

builder.Services.AddSingleton(provider => new DatabaseConnectionFactory(tripDbConnectionString));
builder.Services.AddScoped<TripRepository>();
builder.Services.AddScoped<TripDataService>();

builder.Services.AddServiceModelServices();
builder.Services.AddServiceModelMetadata();

var app = builder.Build();

app.UseServiceModel(serviceBuilder =>
{
    serviceBuilder.AddService<TripDataService>(options =>
    {
        options.DebugBehavior.IncludeExceptionDetailInFaults = true;
    });

    serviceBuilder.AddServiceEndpoint<TripDataService, ITripDataService>(
        new BasicHttpBinding(BasicHttpSecurityMode.None),
        "/TripDataService.svc"
    );

    var serviceMetadataBehavior = app.Services.GetRequiredService<ServiceMetadataBehavior>();
    serviceMetadataBehavior.HttpGetEnabled = true;
});

app.MapGet("/", () => Results.Redirect("/TripDataService.svc"));
app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "TripDataService" }));

app.Run();
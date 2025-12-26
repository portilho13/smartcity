using CoreWCF;
using CoreWCF.Channels;
using CoreWCF.Configuration;
using CoreWCF.Description;
using SmartCity.DataLayer.IoTService.Contracts;
using SmartCity.DataLayer.IoTService.Infrastructure;
using SmartCity.DataLayer.IoTService.Repositories;
using SmartCity.DataLayer.IoTService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:5105");

var iotDbConnectionString = "Host=localhost;Port=5432;Database=smartcity_iot;Username=myuser;Password=mypassword";

builder.Services.AddSingleton(provider => new DatabaseConnectionFactory(iotDbConnectionString));
builder.Services.AddScoped<IoTRepository>();
builder.Services.AddScoped<IoTDataService>();

builder.Services.AddServiceModelServices();
builder.Services.AddServiceModelMetadata();

var app = builder.Build();

app.UseServiceModel(serviceBuilder =>
{
    serviceBuilder.AddService<IoTDataService>(options =>
    {
        options.DebugBehavior.IncludeExceptionDetailInFaults = app.Environment.IsDevelopment();
    });

    serviceBuilder.AddServiceEndpoint<IoTDataService, IIoTDataService>(
        new BasicHttpBinding(BasicHttpSecurityMode.None),
        "/IoTDataService.svc"
    );

    var serviceMetadataBehavior = app.Services.GetRequiredService<ServiceMetadataBehavior>();
    serviceMetadataBehavior.HttpGetEnabled = true;
});

app.MapGet("/", () => Results.Redirect("/IoTDataService.svc"));
app.MapGet("/health", () => Results.Ok(new { 
    status = "healthy", 
    service = "IoTDataService",
    timestamp = DateTime.UtcNow 
}));

Console.WriteLine("IoTDataService starting on http://localhost:5105");
app.Run();
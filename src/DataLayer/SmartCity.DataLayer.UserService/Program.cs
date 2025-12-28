using CoreWCF;
using CoreWCF.Channels;
using CoreWCF.Configuration;
using CoreWCF.Description;
using SmartCity.DataLayer.UserService.Contracts;
using SmartCity.DataLayer.UserService.Infrastructure;
using SmartCity.DataLayer.UserService.Repositories;
using SmartCity.DataLayer.UserService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:5101");

// Hardcoded connection string
var userDbConnectionString = builder.Configuration.GetConnectionString("UserDatabase") 
                             ?? "Host=localhost;Port=5432;Database=smartcity_users;Username=myuser;Password=mypassword";
// Add services
builder.Services.AddSingleton(provider => new DatabaseConnectionFactory(userDbConnectionString));
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<UserDataService>();

builder.Services.AddServiceModelServices();
builder.Services.AddServiceModelMetadata();

var app = builder.Build();

app.UseServiceModel(serviceBuilder =>
{
    serviceBuilder.AddService<UserDataService>(options =>
    {
        options.DebugBehavior.IncludeExceptionDetailInFaults = true;
    });

    serviceBuilder.AddServiceEndpoint<UserDataService, IUserDataService>(
        new BasicHttpBinding(BasicHttpSecurityMode.None),
        "/UserDataService.svc"
    );

    var serviceMetadataBehavior = app.Services.GetRequiredService<ServiceMetadataBehavior>();
    serviceMetadataBehavior.HttpGetEnabled = true;
});

app.MapGet("/", () => Results.Redirect("/UserDataService.svc"));
app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "UserDataService" }));

app.Run();
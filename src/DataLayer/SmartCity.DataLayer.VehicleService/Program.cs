/*
 * ===================================================================================
 * TRABALHO PRÁTICO: Integração de Sistemas de Informação (ISI)
 * -----------------------------------------------------------------------------------
 * Nome: Mario Junior Manhente Portilho
 * Número: a27989
 * Curso: Engenharia de Sistemas Informáticos
 * Ano Letivo: 2025/2026
 * ===================================================================================
 */

using CoreWCF;
using CoreWCF.Channels;
using CoreWCF.Configuration;
using CoreWCF.Description;
using SmartCity.DataLayer.VehicleService.Contracts;
using SmartCity.DataLayer.VehicleService.Infrastructure;
using SmartCity.DataLayer.VehicleService.Repositories;
using SmartCity.DataLayer.VehicleService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:5102");

// Hardcoded connection string
var userDbConnectionString = builder.Configuration.GetConnectionString("VehicleDatabase") ?? "Host=localhost;Port=5432;Database=smartcity_vehicles;Username=myuser;Password=mypassword";

// Add services
builder.Services.AddSingleton(provider => new DatabaseConnectionFactory(userDbConnectionString));
builder.Services.AddScoped<VehicleRepository>();
builder.Services.AddScoped<VehicleDataService>();

builder.Services.AddServiceModelServices();
builder.Services.AddServiceModelMetadata();

var app = builder.Build();

app.UseServiceModel(serviceBuilder =>
{
    serviceBuilder.AddService<VehicleDataService>(options =>
    {
        options.DebugBehavior.IncludeExceptionDetailInFaults = true;
    });

    serviceBuilder.AddServiceEndpoint<VehicleDataService, IVehicleDataService>(
        new BasicHttpBinding(BasicHttpSecurityMode.None),
        "/VehicleDataService.svc"
    );

    var serviceMetadataBehavior = app.Services.GetRequiredService<ServiceMetadataBehavior>();
    serviceMetadataBehavior.HttpGetEnabled = true;
});

app.MapGet("/", () => Results.Redirect("/VehicleDataService.svc"));
app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "VehicleService" }));

app.Run();
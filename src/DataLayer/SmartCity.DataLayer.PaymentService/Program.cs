using CoreWCF;
using CoreWCF.Channels;
using CoreWCF.Configuration;
using CoreWCF.Description;
using SmartCity.DataLayer.PaymentService.Contracts;
using SmartCity.DataLayer.PaymentService.Infrastructure;
using SmartCity.DataLayer.PaymentService.Repositories;
using SmartCity.DataLayer.PaymentService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:5104");

var paymentDbConnectionString = "Host=localhost;Port=5432;Database=smartcity_payments;Username=myuser;Password=mypassword";

builder.Services.AddSingleton(provider => new DatabaseConnectionFactory(paymentDbConnectionString));
builder.Services.AddScoped<PaymentRepository>();
builder.Services.AddScoped<PaymentDataService>();

builder.Services.AddServiceModelServices();
builder.Services.AddServiceModelMetadata();

var app = builder.Build();

app.UseServiceModel(serviceBuilder =>
{
    serviceBuilder.AddService<PaymentDataService>(options =>
    {
        options.DebugBehavior.IncludeExceptionDetailInFaults = app.Environment.IsDevelopment();
    });

    serviceBuilder.AddServiceEndpoint<PaymentDataService, IPaymentDataService>(
        new BasicHttpBinding(BasicHttpSecurityMode.None),
        "/PaymentDataService.svc"
    );

    var serviceMetadataBehavior = app.Services.GetRequiredService<ServiceMetadataBehavior>();
    serviceMetadataBehavior.HttpGetEnabled = true;
});

app.MapGet("/", () => Results.Redirect("/PaymentDataService.svc"));
app.MapGet("/health", () => Results.Ok(new { 
    status = "healthy", 
    service = "PaymentDataService",
    timestamp = DateTime.UtcNow 
}));

Console.WriteLine("PaymentDataService starting on http://localhost:5104");
app.Run();
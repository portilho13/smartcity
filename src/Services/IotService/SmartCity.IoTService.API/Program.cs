using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SmartCity.DataLayer.IoTService.Contracts;
using SmartCity.IoTService.Core.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:5005");

// Configure JWT
var jwtSecret = builder.Configuration["JwtSettings:Secret"];
if (string.IsNullOrEmpty(jwtSecret) || jwtSecret.Length < 32)
{
    throw new InvalidOperationException("JWT Secret must be at least 32 characters long");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

builder.Services.AddAuthorization();

// Configure SOAP Client
builder.Services.AddScoped<IIoTDataService>(provider =>
{
    var soapUrl = builder.Configuration["SoapServices:IoTDataService"] 
        ?? "http://localhost:5105/IoTDataService.svc";
    
    var binding = new BasicHttpBinding
    {
        Security = { Mode = BasicHttpSecurityMode.None },
        MaxReceivedMessageSize = 2147483647,
        MaxBufferSize = 2147483647
    };
    
    var endpoint = new EndpointAddress(soapUrl);
    var factory = new ChannelFactory<IIoTDataService>(binding, endpoint);
    
    return factory.CreateChannel();
});

builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});

// Register services
builder.Services.AddScoped<IIoTService, IoTService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SmartCity IoT Service API",
        Version = "v1",
        Description = "REST API for IoT Service Microservice"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => Results.Redirect("/swagger"));
app.MapGet("/health", () => Results.Ok(new 
{ 
    status = "healthy", 
    service = "IoTService.API",
    timestamp = DateTime.UtcNow 
}));

app.Run();

public partial class Program { }
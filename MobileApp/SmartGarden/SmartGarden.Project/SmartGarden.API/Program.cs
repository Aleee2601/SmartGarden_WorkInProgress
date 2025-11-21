using System.Text;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SmartGarden.API.Services;
using SmartGarden.Core.Interfaces;
using SmartGarden.Data.Extensions;

var builder = WebApplication.CreateBuilder(args);


// Use the extension defined in SmartGarden.Data.Extensions
// Use SQL Server with the "DefaultConnection" connection string
builder.Services.AddDataLayer(builder.Configuration, connectionName: "DefaultConnection", useSqliteDev: false);

// Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger Configuration with JWT Support
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SmartGarden API",
        Version = "v1",
        Description = "API for SmartGarden IoT plant monitoring and watering system"
    });

    // Add JWT Authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your token in the text input below.\n\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...\""
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

// JWT Authentication Configuration (Dual Authentication: Users + Devices)
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var userSecret = jwtSettings["UserSecret"] ?? throw new InvalidOperationException("JWT UserSecret not configured");
var deviceSecret = jwtSettings["DeviceSecret"] ?? throw new InvalidOperationException("JWT DeviceSecret not configured");
var issuer = jwtSettings["Issuer"] ?? "SmartGarden";
var audience = jwtSettings["Audience"] ?? "SmartGarden";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer("UserAuth", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(userSecret)),
        ClockSkew = TimeSpan.Zero
    };
})
.AddJwtBearer("DeviceAuth", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(deviceSecret)),
        ClockSkew = TimeSpan.FromMinutes(5) // Allow time drift for ESP32
    };
});

// Authorization Policies
builder.Services.AddAuthorization(options =>
{
    // Default policy accepts both user and device tokens
    options.DefaultPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
        .AddAuthenticationSchemes("UserAuth", "DeviceAuth")
        .RequireAuthenticatedUser()
        .Build();

    // User-only policy
    options.AddPolicy("UserOnly", policy =>
    {
        policy.AddAuthenticationSchemes("UserAuth");
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("type", "user");
    });

    // Device-only policy
    options.AddPolicy("DeviceOnly", policy =>
    {
        policy.AddAuthenticationSchemes("DeviceAuth");
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("type", "device");
    });

    // User or Device policy
    options.AddPolicy("UserOrDevice", policy =>
    {
        policy.AddAuthenticationSchemes("UserAuth", "DeviceAuth");
        policy.RequireAuthenticatedUser();
        policy.RequireAssertion(context =>
            context.User.HasClaim("type", "user") ||
            context.User.HasClaim("type", "device"));
    });
});

// CORS Configuration
var corsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("SmartGardenCorsPolicy", policy =>
    {
        policy.WithOrigins(corsOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Rate Limiting Configuration
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.Configure<IpRateLimitPolicies>(builder.Configuration.GetSection("IpRateLimitPolicies"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

// Service Registration
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPlantService, PlantService>();
builder.Services.AddScoped<ISensorService, SensorService>();
builder.Services.AddScoped<IWateringService, WateringService>();
builder.Services.AddHttpClient<WateringService>();

// New enhanced services
builder.Services.AddScoped<IDeviceService, DeviceService>();
builder.Services.AddScoped<IDeviceAuthService, DeviceAuthService>();
builder.Services.AddScoped<IAlertService, AlertService>();

// Plant Info Service (External API Integration)
builder.Services.AddHttpClient<IPlantInfoService, PlantInfoService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("User-Agent", "SmartGarden/1.0");
});


// Background Services
builder.Services.AddHostedService<AutoWateringBackgroundService>();

var app = builder.Build();

// Middleware Pipeline
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "SmartGarden API v1");
    options.RoutePrefix = string.Empty; // Serve Swagger at root
});

// Rate Limiting Middleware
app.UseIpRateLimiting();

// CORS Middleware
app.UseCors("SmartGardenCorsPolicy");

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

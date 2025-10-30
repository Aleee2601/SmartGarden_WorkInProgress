using SmartGarden.API.Services;
using SmartGarden.Core.Interfaces;
using SmartGarden.Data.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Use the extension defined in SmartGarden.Data.Extensions
builder.Services.AddDataLayer(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IPlantService, PlantService>();
builder.Services.AddScoped<ISensorService, SensorService>();
builder.Services.AddScoped<IWateringService, WateringService>();
builder.Services.AddHttpClient<WateringService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();

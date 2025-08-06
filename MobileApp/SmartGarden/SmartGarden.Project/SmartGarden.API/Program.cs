using SmartGarden.API.Services;
using SmartGarden.Core.Interfaces;
using SmartGarden.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSmartGardenData(builder.Configuration.GetConnectionString("DefaultConnection")!);

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

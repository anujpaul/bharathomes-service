using Azure.Identity;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("AzureSqlDb");
string cosmosAccount = builder.Configuration["CosmosAccount"]!;
string cosmosDbName = builder.Configuration["CosmosDbName"]!;
string cosmosContainerName = builder.Configuration["CosmosContainerName"]!;
string cosmosKey = builder.Configuration["CosmosKey"]!;

var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() 
                     ?? Array.Empty<string>();


builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(allowedOrigins) // Your Angular port
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});


builder.Services.AddSingleton<Container>(
    sp =>
    {
        return new CosmosClient(cosmosAccount, cosmosKey).GetContainer(cosmosDbName, cosmosContainerName);
    }
);

builder.Services.AddSingleton<CosmosDbService>();

builder.Services.AddScoped<ImageService>();

var app = builder.Build();

app.UseCors();
app.MapControllers();

app.Run();

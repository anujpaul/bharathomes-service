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
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins(allowedOrigins) // Your Angular port
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
            //   .WithHeaders("Content-Type", "Authorization", "X-User-Id")
              ;
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

app.UseCors("AllowAngular");
app.MapControllers();

app.Run();

using Azure.Identity;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;

string account = "https://cosmosaccountap.documents.azure.com:443/";
string key = "";
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000",
                            "https://bharathomes.azurewebsites.net") // Your Angular port
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var connectionString = builder.Configuration.GetConnectionString("AzureSqlDb");

// 2. Register DbContext with Azure Managed Identity
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(connectionString, sqlOptions => 
    {
        // Recommended for cloud-native apps: automatically retries on transient failures
        sqlOptions.EnableRetryOnFailure();
    });
});

builder.Services.AddSingleton<CosmosClient>(
    sp =>
    {
        return new CosmosClient(account, new DefaultAzureCredential());
    }
);

builder.Services.AddSingleton<CosmosDbService>();

builder.Services.AddScoped<ImageService>();

var app = builder.Build();


app.UseCors();
app.MapControllers();
// app.MapGet("/", () => "Hello World!");

app.Run();

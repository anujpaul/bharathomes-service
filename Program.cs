using Azure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;


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

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://accounts.google.com";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "https://accounts.google.com",
            ValidateAudience = true,
            // Your Google Client ID
            ValidAudience = "952596507071-nhso45200fv957edkhjalcn8ihh6ll32.apps.googleusercontent.com",
            ValidateLifetime = true,
        };
    });

builder.Services.AddAuthorization();



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

// Make sure these are in the right order
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

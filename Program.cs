using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Azure.Cosmos;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
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
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Fetch Google public keys once at startup, cached automatically
var googleConfigManager = new ConfigurationManager<OpenIdConnectConfiguration>(
    "https://accounts.google.com/.well-known/openid-configuration",
    new OpenIdConnectConfigurationRetriever());

var googleConfig = await googleConfigManager.GetConfigurationAsync();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuers = new[]
            {
                "https://accounts.google.com",
                builder.Configuration["Jwt:Issuer"]   // "bharathomes-service"
            },
            ValidateAudience = true,
            ValidAudiences = new[]
            {
                // Google Client ID
                "952596507071-nhso45200fv957edkhjalcn8ihh6ll32.apps.googleusercontent.com",
                // Your local JWT audience
                builder.Configuration["Jwt:Audience"]  // "bharathomes-app"
            },
            ValidateLifetime = true,
            // Resolve signing keys for both Google (RS256) and local (HS256)
            IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
            {
                var keys = new List<SecurityKey>();

                // Google RS256 public keys
                keys.AddRange(googleConfig.SigningKeys);

                // Your local HS256 symmetric key
                var secret = builder.Configuration["Jwt:Secret"];
                if (!string.IsNullOrEmpty(secret))
                    keys.Add(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)));

                return keys;
            }
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<TokenService>();

builder.Services.AddSingleton<Container>(sp =>
    new CosmosClient(cosmosAccount, cosmosKey)
        .GetContainer(cosmosDbName, cosmosContainerName));

builder.Services.AddSingleton<CosmosDbService>();
builder.Services.AddScoped<ImageService>();

var app = builder.Build();

app.UseCors("AllowAngular");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
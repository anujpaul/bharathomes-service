using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var SqlConnection = builder.Configuration.GetConnectionString("SqlConnection");

var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>()
                     ?? Array.Empty<string>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });

builder.Services.AddDbContext<SqlDbContext>(options =>
// options.UseSqlServer(SqlConnection));
    options.UseNpgsql(SqlConnection));

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
builder.Services.AddScoped<ImageService>();

builder.Services.AddMemoryCache();
builder.Services.AddScoped<OtpService>();
//builder.Services.AddScoped<IEmailService, ConsoleEmailService>(); // swap later
builder.Services.AddScoped<IEmailService, SmtpEmailService>();

// builder.Logging.ClearProviders();

// builder.Logging.AddSimpleConsole(options =>
// {
//     options.TimestampFormat = "yyyy-MM-dd HH:mm:ss ";
//     options.IncludeScopes = true;
//     options.SingleLine = true;
// });

var app = builder.Build();

app.UseCors("AllowAngular");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.Use(async (context, next) =>
    {
        var principal = new { userId = "mock-id", userDetails = "dev@example.com", userRoles = new[] { "authenticated" }, identityProvider = "google" };
        var json = JsonSerializer.Serialize(principal);
        var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
        context.Request.Headers["X-MS-CLIENT-PRINCIPAL"] = encoded;
        context.Request.Headers["X-MS-CLIENT-PRINCIPAL-NAME"] = "dev@example.com";
        context.Request.Headers["X-MS-CLIENT-PRINCIPAL-ID"] = "mock-id";
        await next();
    });
}

app.Run();
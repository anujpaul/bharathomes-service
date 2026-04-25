using System.Security.Cryptography;
using System.Text;

public class UserService
{
    private readonly CosmosDbService _cosmosService;

    private readonly ILogger<UserService> _logger;

    public UserService(CosmosDbService cosmosService, ILogger<UserService> logger)
    {
        _cosmosService = cosmosService;
        _logger = logger;
    }

    public async Task<UserProfile?> ValidateCredentials(string email, string password)
    {
        _logger.LogInformation($"User : {email} {password}");
        var user = await _cosmosService.ReadItemByEmailAsync<UserProfile>(email);
        _logger.LogInformation($"Is user empty? : {user}");
        if (user == null) return null;

        _logger.LogInformation($"Password in DB: {user.PasswordHash}");
        _logger.LogInformation($"Password Hash  {HashPassword(password)}");
        // Compare hashed password
        var hash = HashPassword(password);
        return hash == user.PasswordHash ? user : null;
    }

    public async Task<UserProfile?> CreateLocalUser(string? email, string password, string name)
    {   
        email = email?.Trim().ToLower();

        var existing = await _cosmosService.ReadItemByEmailAsync<UserProfile>(email);
        
        _logger.LogInformation($"User : {existing}");
        if (existing != null)
        {
            _logger.LogInformation($"Provider : {existing.Provider}");
            if (existing.Provider != "local")
            {
                // Convert social user → hybrid user
                existing.PasswordHash = HashPassword(password);
                existing.Provider = "hybrid"; // or keep both flags
                _logger.LogInformation("Converting user to hybrid");
                await _cosmosService.UpdateItemAsync<UserProfile>(existing);
                return existing;
            }
            return null;
        }

        var user = new UserProfile
        {
            Id = Guid.NewGuid().ToString(),
            Email = email,
            Name = name,
            PasswordHash = HashPassword(password),
            Provider = "local"
        };
        _logger.LogInformation($"Creating User");
        await _cosmosService.CreateItemAsync<UserProfile>(user);
        return user;
    }

    private static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}
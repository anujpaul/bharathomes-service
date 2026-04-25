using System.Security.Cryptography;
using System.Text;

public class UserService
{
    private readonly CosmosDbService _cosmosService;

    public UserService(CosmosDbService cosmosService)
    {
        _cosmosService = cosmosService;
    }

    public async Task<UserProfile?> ValidateCredentials(string email, string password)
    {
        System.Console.WriteLine($"User : {email} {password}");
        var user = await _cosmosService.ReadItemByEmailAsync<UserProfile>(email);
        System.Console.WriteLine($"Is user empty? : {user}");
        if (user == null) return null;

        System.Console.WriteLine($"Password in DB: {user.PasswordHash}");
        System.Console.WriteLine($"Password Hash  {HashPassword(password)}");
        // Compare hashed password
        var hash = HashPassword(password);
        return hash == user.PasswordHash ? user : null;
    }

    public async Task<UserProfile> CreateLocalUser(string email, string password, string name)
    {
        

        var existing = await _cosmosService.ReadItemByEmailAsync<UserProfile>(email);
        
        System.Console.WriteLine($"User : {existing}");
        if (existing != null)
            throw new Exception("Email already registered");

        var user = new UserProfile
        {
            Id = Guid.NewGuid().ToString(),
            Email = email,
            Name = name,
            PasswordHash = HashPassword("password"),
            Provider = "local"
        };
        System.Console.WriteLine($"Creating User");
        await _cosmosService.CreateItemAsyc<UserProfile>(user);
        return user;
    }

    private static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}
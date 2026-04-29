using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;

public class UserService
{
    private readonly SqlDbContext _db;
    private readonly ILogger<UserService> _logger;

    public UserService(SqlDbContext db, ILogger<UserService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<UserProfile?> ValidateCredentials(string email, string password)
    {
        var user = await _db.UserProfiles
            .FirstOrDefaultAsync(u => u.Email == email);

        _logger.LogInformation($"Is user empty? : {user}");

        if (user == null)
            return null;

        var hash = HashPassword(password);
        return hash == user.PasswordHash ? user : null;
    }

    public async Task<UserProfile?> CreateLocalUser(string? email, string password, string name)
    {
        email = email?.Trim().ToLower();

        if (email == null)
            return null;

        var existing = await _db.UserProfiles
            .FirstOrDefaultAsync(u => u.Email == email);

        _logger.LogInformation($"User : {existing}");

        if (existing != null)
        {
            _logger.LogInformation($"Provider : {existing.Provider}");
            if (existing.Provider != "local")
            {
                // Convert social user → hybrid user
                existing.PasswordHash = HashPassword(password);
                existing.Provider = "hybrid";
                _logger.LogInformation("Converting user to hybrid");
                await _db.SaveChangesAsync();
                return existing;
            }
            return null; // email already registered
        }

        var user = new UserProfile
        {
            Id = Guid.NewGuid().ToString(),
            Email = email,
            Name = name,
            PasswordHash = HashPassword(password),
            Provider = "local",
            AccountStatus = true
        };

        _logger.LogInformation("Creating User");
        _db.UserProfiles.Add(user);
        await _db.SaveChangesAsync();
        return user;
    }

    public async Task InitiatePasswordReset(string email)
    {
        throw new NotImplementedException();
    }

    private static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;

public class UserService
{
    private readonly SqlDbContext _db;
    private readonly ILogger<UserService> _logger;

    // Add this result type at the top of the file (or a separate file)
    public enum RegisterStatus { Created, EmailTaken, RequiresOtp }
    public record RegisterResult(RegisterStatus Status, UserProfile? User = null);

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

    public async Task<RegisterResult> CreateLocalUser(string? email, string password, string name)
    {
        email = email?.Trim().ToLower();
        if (email == null) return new RegisterResult(RegisterStatus.EmailTaken);

        var existing = await _db.UserProfiles.FirstOrDefaultAsync(u => u.Email == email);

        if (existing != null)
        {
            // Social account exists — don't touch it yet, demand OTP first
            if (existing.Provider != "local")
                return new RegisterResult(RegisterStatus.RequiresOtp);

            return new RegisterResult(RegisterStatus.EmailTaken); // local duplicate
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

        _db.UserProfiles.Add(user);
        await _db.SaveChangesAsync();
        return new RegisterResult(RegisterStatus.Created, user);
    }

    // Called AFTER OTP is verified
    public async Task<UserProfile?> MergeAfterOtp(string email, string password, string name)
    {
        var existing = await _db.UserProfiles.FirstOrDefaultAsync(u => u.Email == email);
        if (existing == null || existing.Provider == "local") return null;

        existing.PasswordHash = HashPassword(password);
        existing.Provider = "hybrid";
        if (string.IsNullOrWhiteSpace(existing.Name)) existing.Name = name;

        await _db.SaveChangesAsync();
        return existing;
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
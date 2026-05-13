using System.Security.Claims;
using bharathome_api.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly SqlDbContext _db;
    private readonly ILogger<UserController> _logger;
    private readonly ImageService _imageService;

    public UserController(SqlDbContext db, ILogger<UserController> logger, ImageService imageService)
    {
        _db = db;
        _logger = logger;
        _imageService = imageService;
    }

    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                   ?? User.FindFirst("sub")?.Value;
        var userName = User.FindFirst(ClaimTypes.GivenName)?.Value
                        ?? User.FindFirst("name")?.Value;
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value
                        ?? User.FindFirst("email")?.Value;
        var iss = User.FindFirst("iss")?.Value;

        var provider = iss switch
        {
            string s when s.Contains("accounts.google.com") => "google",
            string s when s.Contains("login.microsoftonline.com") => "microsoft",
            _ => "local"
        };

        userEmail = userEmail?.Trim().ToLower();

        _logger.LogInformation("Name: {name}, Email: {email}, Provider: {provider}, iss: {iss}",
            userName, userEmail, provider, iss);

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userEmail))
            return Unauthorized();

        var profile = await _db.UserProfiles
            .Include(u => u.Kyc)
            .FirstOrDefaultAsync(u => u.Email == userEmail);

        // First-time OAuth sign-in: there's no row in user_profiles yet
        // because OAuth users don't go through /api/auth/register (which
        // is the path that creates the row for local sign-ups). Create
        // it now from the JWT claims so the user has a profile to attach
        // KYC, listings, and a role to.
        if (profile == null && provider != "local")
        {
            profile = new UserProfile
            {
                Id = userId!,
                Name = userName ?? "Unknown",
                Email = userEmail!,
                Provider = provider,
                AccountStatus = true,
                CreatedAt = DateTime.UtcNow,
            };
            _db.UserProfiles.Add(profile);
            await _db.SaveChangesAsync();
            _logger.LogInformation(
                "Auto-created profile for OAuth user {Email} via {Provider}",
                userEmail, provider);
        }
        // Local user with no row would mean they never completed registration;
        // returning 404 is the right answer.
        else if (profile == null)
        {
            return NotFound();
        }
        // User registered locally and is now signing in via Google/Microsoft.
        // Promote provider to "hybrid" so we know both paths are valid.
        else if (provider != "local" && profile.Provider == "local")
        {
            _logger.LogInformation(
                "Promoting {Email} from local to hybrid (signed in via {Provider})",
                userEmail, provider);
            profile.Provider = "hybrid";
            await _db.SaveChangesAsync();
        }

        var dto = new UserProfileDto
        {
            Id = profile.Id,
            Name = profile.Name,
            Email = profile.Email,
            Phone = profile.Phone,
            UserPhoto = profile.UserPhoto,
            UserRole = profile.UserRole,
            AccountStatus = profile.AccountStatus,
            IsPaid = profile.IsPaid,
            SubscriptionExpiry = profile.SubscriptionExpiry,
            SubscriptionStartedAt = profile.SubscriptionStartedAt,
            CurrentPlanCode = profile.CurrentPlanCode,
            CurrentPlanTier = profile.CurrentPlanTier,
            Provider = profile.Provider,
            KycStatus = profile.Kyc != null? profile.Kyc.Status.ToString().ToLower():"pending"
        };
        

        profile.PasswordHash = null;
        return Ok(dto);
    }

    [HttpPut("profile")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile([FromBody] UserProfile userProfile)
    {
        var userEmail = userProfile.Email?.Trim().ToLower();

        var profile = await _db.UserProfiles
            .FirstOrDefaultAsync(u => u.Email == userEmail);

        if (profile == null)
            return NotFound();

        profile.Name = userProfile.Name;
        profile.Email = userProfile.Email;
        profile.Phone = userProfile.Phone;
        profile.UserRole = userProfile.UserRole;

        await _db.SaveChangesAsync();

        profile.PasswordHash = null;
        return Ok(profile);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProfileById(string id)
    {
        var profile = await _db.UserProfiles.FindAsync(id);

        if (profile == null)
            return NotFound();

        profile.PasswordHash = null;
        return Ok(profile);
    }

    [HttpPost("upload-photo")]
    [Authorize]
    public async Task<IActionResult> UploadPhoto(IFormFile file)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(userId)) return Unauthorized();
        if (file == null || file.Length == 0) return BadRequest(new { message = "No file provided" });

        _logger.LogInformation($"File Information {file} type: {file.ContentType}");

        var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/webp", "image/heic", "image/heif" };
        if (!allowedTypes.Contains(file.ContentType))
            return BadRequest(new { message = "Only JPEG, PNG and WebP allowed" });

        if (file.Length > 2 * 1024 * 1024)
            return BadRequest(new { message = "Image must be under 2MB" });

        var ext = Path.GetExtension(file.FileName);
        var fileName = $"{userId}{ext}";

        using var stream = file.OpenReadStream();
        var photoUrl = await _imageService.UploadImageAsync(stream, "Users", fileName, file.ContentType);

        var profile = await _db.UserProfiles.FindAsync(userId);
        if (profile == null) return NotFound();

        profile.UserPhoto = photoUrl;
        await _db.SaveChangesAsync();

        return Ok(new { userPhoto = photoUrl });
    }

    [HttpPatch("type")]
    [Authorize]
    public async Task<IActionResult> UpdateUserType([FromBody] UpdateTypeRequest request)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized();

        var user = await _db.UserProfiles.FirstOrDefaultAsync(u => u.Id == userId || u.Email == userId);
        if (user == null) return NotFound();
       

        var allowed = new[] { "buyer", "seller", "agent", "hybrid" };
        if (!allowed.Contains(request.UserType.ToLower()))
            return BadRequest(new { message = "Invalid user type" });

        user.UserRole = request.UserType.ToLower();

        _logger.LogInformation($"Updating User {userId}ile {user}");
        await _db.SaveChangesAsync();

        return Ok(new { userType = user.UserRole });
    }

    public record UpdateTypeRequest(string UserType);
}
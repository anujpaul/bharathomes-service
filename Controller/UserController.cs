using System.Security.Claims;
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
            .FirstOrDefaultAsync(u => u.Email == userEmail);

        if (provider != "local")
        {
            if (profile == null)
            {
                profile = new UserProfile
                {
                    Id = userId!,
                    Name = userName ?? "Unknown",
                    Email = userEmail ?? "Unknown",
                    Provider = provider,
                    AccountStatus = true
                };
                _db.UserProfiles.Add(profile);
                await _db.SaveChangesAsync();
            }
            else
            {
                _logger.LogInformation("Updating easy auth to hybrid");
                profile.Provider = "hybrid";
                await _db.SaveChangesAsync();
            }
        }

        if (profile == null)
            return NotFound();

        profile.PasswordHash = null;
        return Ok(profile);
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

        await _db.SaveChangesAsync();

        profile.PasswordHash = null;
        return Ok(profile);
    }

    [HttpGet("profile/{id}")]
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
}
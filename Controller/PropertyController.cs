using bharathome_api.DTOs;
using bharathome_api.Interfaces;
using bharathome_api.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Net.Cache;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]

public class PropertyController : ControllerBase
{
    private readonly ILogger<PropertyController> _logger;
    private readonly ImageService _imageService;

    private readonly IPropertyService _propertyService;


    private readonly SqlDbContext _db;
    public PropertyController(ImageService imageService, ILogger<PropertyController> logger, SqlDbContext db, IPropertyService propertyService)
    {
        _imageService = imageService;
        _logger = logger;
        _db = db;
        _propertyService = propertyService;
    }

    [HttpGet]
    public async Task<IActionResult> Properties(
        [FromQuery] string? intent = null,
        [FromQuery] string? type = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] string? city = null)
    {
        _logger.LogInformation(
            "Properties called: intent={Intent} type={Type} min={Min} max={Max} city={City}",
            intent, type, minPrice, maxPrice, city);

        // Build the query incrementally — every filter is optional, so each
        // becomes a Where clause only when the caller supplied it. Intent
        // and type/city use case-insensitive equality (Postgres is case-
        // sensitive by default; ToLower keeps it predictable).
        var query = _db.Properties.AsQueryable();

        if (!string.IsNullOrWhiteSpace(intent))
        {
            var normalized = intent.Trim().ToLowerInvariant();
            query = query.Where(p => p.ListingIntent.ToLower() == normalized);
        }

        if (!string.IsNullOrWhiteSpace(type))
        {
            var normalized = type.Trim().ToLowerInvariant();
            query = query.Where(p => p.Type.ToLower() == normalized);
        }

        if (minPrice.HasValue)
            query = query.Where(p => p.Price >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(p => p.Price <= maxPrice.Value);

        if (!string.IsNullOrWhiteSpace(city))
        {
            var normalized = city.Trim().ToLowerInvariant();
            query = query.Where(p => p.City.ToLower() == normalized);
        }

        var properties = await query
            .Select(p => new
            {
                p.Id,
                p.Title,
                p.Price,
                p.Location,
                p.City,
                p.Beds,
                p.Baths,
                p.Sqft,
                p.Type,
                p.ListingIntent,
                p.IsFeatured,
                p.ExpresswayProximity,
                Images = p.Images.OrderBy(i => i.SortOrder).Select(i => i.Url).ToList(),
                Amenities = p.Amenities.Select(a => a.Name).ToList(),
            })
            .ToListAsync();

        return Ok(properties);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPropertyById(string id)
    {
        _logger.LogInformation("here");

        var property = await _db.Properties
    .Where(p => p.Id == id)
    .Select(p => new
    {
        p.Id,
        p.Title,
        p.Price,
        p.Location,
        p.City,
        p.Beds,
        p.Baths,
        p.Sqft,
        p.Type,
        p.IsFeatured,
        p.ExpresswayProximity,
        p.IsReraRegistered,
        p.ReraRegistrationNumber,
        p.ReraDocumentUrl,
        p.VastuOrientation,
        p.ListerId,
        p.BuiltYear,
        listedSince = (DateTime.UtcNow - p.CreatedAt).Days + " days",
        Images = p.Images.OrderBy(i => i.SortOrder).Select(i => i.Url).ToList(),
        Amenities = p.Amenities.Select(a => a.Name).ToList(),
        Agents = p.PropertyAgents.Select(pa => new
        {
            pa.Agent.Id,
            pa.Agent.UserProfile.Name,
            pa.Agent.UserProfile.Email,
            pa.Agent.UserProfile.Phone,
            pa.Agent.UserProfile.UserPhoto,
            pa.Agent.Rating,
            pa.Agent.ListingsCount,
            pa.Agent.Specialization
        }).ToList()
    })
    .FirstOrDefaultAsync();

        if (property == null)
            return NotFound(new { message = "Property not found" });
        return Ok(property);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePropertyAsync(string id)
    {
        var property = await _db.Properties.FindAsync(id);

        if (property == null)
            return NotFound(new { message = "Property not found" });

        _db.Properties.Remove(property);
        await _db.SaveChangesAsync();

        return Ok(new { message = "Property deleted" });
    }



    [HttpPost("property")]
    [Authorize]
    public async Task<IActionResult> CreatePropertyAsync([FromBody] CreatePropertyDto dto)
    {

        var listerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (listerId == null) return Unauthorized();

        // IMPORTANT: include Kyc — FindAsync does NOT load navigation properties,
        // and we need user.Kyc.Status below.
        var user = await _db.UserProfiles
            .Include(u => u.Kyc)
            .FirstOrDefaultAsync(u => u.Id == listerId);
        if (user == null) return Unauthorized();

        if (user.Kyc == null || user.Kyc.Status != KycStatus.Verified)
            return BadRequest(new
            {
                code = "KYC_REQUIRED",
                message = "Complete identity verification before listing a property."
            });
        var listingCount = await _db.Properties
        .CountAsync(p => p.ListerId == listerId);

        var limit = GetListingLimit(user);
        if (listingCount >= limit)
            return BadRequest(new
            {
                code = "LISTING_LIMIT_REACHED",
                message = $"You have reached your limit of {limit} listings.",
                upgradeRequired = !user.IsPaid
            });
        dto.ListerId = listerId;
        var id = await _propertyService.CreatePropertyAsync(dto);
        return Ok(new { id });

    }

    private int GetListingLimit(UserProfile user)
    {
        // Active subscription? Apply the tier's limit.
        var subscriptionActive = user.IsPaid &&
            (user.SubscriptionExpiry == null || user.SubscriptionExpiry > DateTime.UtcNow);
        if (subscriptionActive)
        {
            // Pro tier — effectively unlimited.
            // Basic tier — 10 active listings.
            // Default to Pro behavior for legacy paid users that pre-date the
            // CurrentPlanTier column (they paid under the old single-tier flow).
            return user.CurrentPlanTier?.ToLowerInvariant() switch
            {
                "basic" => 10,
                "pro"   => 200,
                _       => 200,
            };
        }

        var role = user.UserRole.ToLower();

        // Free agents / developers / builders — 10 for first 90 days
        if (role is "agent" or "developer" or "builder")
        {
            var daysSinceJoined = (DateTime.UtcNow - user.CreatedAt).TotalDays;
            return daysSinceJoined <= 90 ? 10 : 0;  // 0 forces upgrade after trial
        }

        // Free owners — 2 max
        return 2;
    }


    // [HttpPost("createUser")]
    // public async Task<IActionResult> CreateUserAsync([FromBody] UserProfile user)
    // {
    //     _logger.LogInformation($"User called : {System.Text.Json.JsonSerializer.Serialize(user)}");
    //     return Ok(await _cosmosService.CreateItemAsync<UserProfile>(user));
    // }
    [HttpPost("{id}/images")]
    [Authorize]
    public async Task<IActionResult> UploadImage(string id, IFormFile file)
    {
        var property = await _db.Properties.FindAsync(id);
        if (property == null) return NotFound(new { message = "Property not found" });

        if (file == null || file.Length == 0)
            return BadRequest(new { message = "No file provided" });

        var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/webp" };
        if (!allowedTypes.Contains(file.ContentType))
            return BadRequest(new { message = "Only JPEG, PNG and WebP allowed" });

        if (file.Length > 10 * 1024 * 1024)
            return BadRequest(new { message = "Image must be under 10MB" });

        var ext = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid()}{ext}";

        using var stream = file.OpenReadStream();
        var url = await _imageService.UploadImageAsync(stream, $"Properties/{id}", fileName, file.ContentType);

        var nextOrder = property.Images?.Count ?? 0;
        var image = new PropertyImage { Url = url, SortOrder = nextOrder, PropertyId = id };
        _db.PropertyImages.Add(image);
        await _db.SaveChangesAsync();

        return Ok(new { url, id = image.Id, order = image.SortOrder });
    }

    [HttpPost("{id}/media")]
    [Authorize]
    public async Task<IActionResult> UploadMedia(string id, IFormFileCollection files)
    {
        var property = await _db.Properties.FindAsync(id);
        if (property == null) return NotFound(new { message = "Property not found" });

        if (files == null || files.Count == 0)
            return BadRequest(new { message = "No files provided" });

        var allowedImageTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/webp" };
        var allowedVideoTypes = new[] { "video/mp4", "video/webm", "video/ogg", "video/quicktime" };
        const long maxImageSize = 10 * 1024 * 1024;      // 10 MB
        const long maxVideoSize = 100 * 1024 * 1024;    // 100 MB (adjust as needed)

        var uploadedUrls = new List<string>();
        int currentOrder = property.Images?.Count ?? 0;  // store both images & videos in `Images` list

        foreach (var file in files)
        {
            if (file.Length == 0) continue;

            bool isImage = allowedImageTypes.Contains(file.ContentType);
            bool isVideo = allowedVideoTypes.Contains(file.ContentType);
            if (!isImage && !isVideo)
                return BadRequest(new { message = $"File {file.FileName} has invalid type. Allowed: images (JPEG, PNG, WebP) or videos (MP4, WebM, OGG, MOV)." });

            long maxSize = isImage ? maxImageSize : maxVideoSize;
            if (file.Length > maxSize)
                return BadRequest(new { message = $"File {file.FileName} exceeds {(isImage ? "10MB" : "100MB")} limit." });

            var ext = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{ext}";
            var subFolder = isImage ? "Properties" : "Videos"; // optional: organise in different folders

            using var stream = file.OpenReadStream();
            var url = await _imageService.UploadImageAsync(stream, $"{subFolder}/{id}", fileName, file.ContentType);
            // If your `_imageService` is generic, keep using it. If not, create a separate video upload service.

            var media = new PropertyImage { Url = url, SortOrder = currentOrder++, PropertyId = id };
            _db.PropertyImages.Add(media);
            uploadedUrls.Add(url);
        }

        await _db.SaveChangesAsync();
        return Ok(uploadedUrls); // returns string[] – matches frontend expectation
    }

    [HttpPatch("{id}/reorder-images")]
    [Authorize]
    public async Task<IActionResult> ReorderImages(string id, [FromBody] ReorderImagesRequest request)
    {
        var property = await _db.Properties.FindAsync(id);
        if (property == null) return NotFound();

        foreach (var img in request.Images)
        {
            var imageEntity = await _db.PropertyImages
                .FirstOrDefaultAsync(i => i.Url == img.Url && i.PropertyId == id);
            if (imageEntity != null)
                imageEntity.SortOrder = img.SortOrder;
        }
        await _db.SaveChangesAsync();
        return Ok();
    }

    public class ReorderImagesRequest
    {
        public List<ImageOrder> Images { get; set; }
    }

    public class ImageOrder
    {
        public string Url { get; set; }
        public int SortOrder { get; set; }
    }

    [HttpDelete("{id}/images")]
    [Authorize]
    public async Task<IActionResult> DeleteImage([FromBody] DeleteImageRequest request, string id)
    {
        _logger.LogInformation($"Url = {request.Url}, Id: {id}");

        var image = await _db.PropertyImages
            .FirstOrDefaultAsync(i => i.Url == request.Url && i.PropertyId == id);
        if (image == null) return NotFound();

        _db.PropertyImages.Remove(image);
        await _db.SaveChangesAsync();
        return Ok();
    }

    [HttpPatch("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateProperty(string id, [FromBody] Property updatedProperty)
    {

        _logger.LogInformation($"Title: {updatedProperty.Id}, Price: {updatedProperty.Price} Title: {updatedProperty.Title}======================================");

        var property = await _db.Properties.FindAsync(id);
        if (property == null) return NotFound(new { message = "Property not found" });
        property.Title = updatedProperty.Title;
        property.Price = updatedProperty.Price;
        property.Location = updatedProperty.Location;
        property.City = updatedProperty.City;
        property.Beds = updatedProperty.Beds;
        property.Baths = updatedProperty.Baths;
        property.Sqft = updatedProperty.Sqft;
        property.Type = updatedProperty.Type;
        property.IsFeatured = updatedProperty.IsFeatured;
        property.ExpresswayProximity = updatedProperty.ExpresswayProximity;
        property.IsReraRegistered = updatedProperty.IsReraRegistered;
        property.ReraRegistrationNumber = updatedProperty.ReraRegistrationNumber;
        property.VastuOrientation = updatedProperty.VastuOrientation;
        await _db.SaveChangesAsync();
        return Ok(property);
    }

    [HttpPost("{propertyId}/rera-doc")]
    [Authorize]
    public async Task<IActionResult> UploadReraDocument(string propertyId, IFormFile file)
    {
        // Resolve the property (need the entity to write the URL back to it).
        var property = await _db.Properties.FindAsync(propertyId);
        if (property == null)
            return NotFound(new { message = "Property not found" });

        // Only the lister should be able to attach a RERA certificate.
        // (Same trust model as UploadImage today; tighten if you want
        // co-listing agents to upload too.)
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId) || property.ListerId != userId)
            return Forbid();

        if (file == null || file.Length == 0)
            return BadRequest(new { message = "No file provided" });

        // RERA certificates are typically PDFs; accept common image formats too
        // for users who only have a photo of the certificate.
        var allowedTypes = new[]
        {
            "application/pdf",
            "image/jpeg", "image/jpg", "image/png", "image/webp"
        };
        if (!allowedTypes.Contains(file.ContentType))
            return BadRequest(new { message = "Only PDF, JPEG, PNG or WebP allowed" });

        if (file.Length > 5 * 1024 * 1024)
            return BadRequest(new { message = "File must be under 5MB" });

        // Filename keeps the original extension so the blob serves with the
        // right content type via the URL when the user opens it later.
        var ext = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid()}{ext}";

        using var stream = file.OpenReadStream();
        var url = await _imageService.UploadImageAsync(
            stream, $"Properties/{propertyId}/rera", fileName, file.ContentType);

        // Persist the URL on the property so subsequent GETs can surface it.
        property.ReraDocumentUrl = url;
        await _db.SaveChangesAsync();

        return Ok(new { url });
    }

}




public class DeleteImageRequest
{
    public string Url { get; set; } = string.Empty;
}
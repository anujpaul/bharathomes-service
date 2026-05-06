using bharathome_api.DTOs;
using bharathome_api.Interfaces;
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
    public async Task<IActionResult> Properties()
    {
        _logger.LogInformation("Properties called");
        // var properties = await _cosmosService.ReadItemsAsync<Property>();
        var properties = await _db.Properties
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
        p.VastuOrientation,
        p.CreatedAt,
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
        var user = await _db.UserProfiles.FindAsync(listerId);
        if (listerId == null || user == null) return Unauthorized();

        if (user.KycStatus != KycStatus.Verified)
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
        // Paid users — unlimited
        if (user.IsPaid && (user.SubscriptionExpiry == null || user.SubscriptionExpiry > DateTime.UtcNow))
            return 200; // effectively unlimited

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


}




public class DeleteImageRequest
{
    public string Url { get; set; } = string.Empty;
}
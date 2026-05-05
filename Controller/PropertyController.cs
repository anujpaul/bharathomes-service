using System.Data.Common;
using System.Net.Cache;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using bharathome_api.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")] 

public class PropertyController : ControllerBase
{
    private readonly ILogger<PropertyController> _logger;
    private readonly ImageService _imageService;
    

    private readonly SqlDbContext _db;
    public PropertyController(ImageService imageService, ILogger<PropertyController> logger, SqlDbContext db)
    {
        _imageService = imageService;
        _logger = logger;
        _db = db;
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
        // ✅ Single async DB call to get all valid agent IDs at once
        var validAgentIds = await _db.Agents
            .Where(a => dto.AgentId.Contains(a.Id))
            .Select(a => a.Id)
            .ToListAsync();

        var property = new Property
        {
            Id = Guid.NewGuid().ToString(),
            Title = dto.Title,
            Price = dto.Price,
            Location = dto.Location,
            City = dto.City,
            Beds = dto.Beds,
            Baths = dto.Baths,
            Sqft = dto.Sqft,
            Type = dto.Type,
            IsFeatured = dto.IsFeatured,
            ExpresswayProximity = dto.ExpresswayProximity,
            ListerId = dto.ListerId,
            Images = dto.Images.Select(url => new PropertyImage { Url = url }).ToList(),
            Amenities = dto.Amenities.Select(a => new PropertyAmenity { Name = a }).ToList(),
            PropertyAgents = validAgentIds
                .Select(id => new PropertyAgent { AgentId = id })
                .ToList(),
        };

        _db.Properties.Add(property);
        await _db.SaveChangesAsync();
        return Ok(new { property.Id });
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
        _logger.LogInformation($"Image Id : {id}.==============================");
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
        _logger.LogInformation($"Url = {request.Url}, Id: {id}===========================================================================");

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
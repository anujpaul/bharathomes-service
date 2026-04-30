using System.Data.Common;
using System.Net.Cache;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
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
                Images = p.Images.OrderBy(i => i.Order).Select(i => i.Url).ToList(),
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
        Images = p.Images.OrderBy(i => i.Order).Select(i => i.Url).ToList(),
        Amenities = p.Amenities.Select(a => a.Name).ToList(),
        Agents = p.PropertyAgents.Select(pa => new
        {
            pa.Agent.Id,
            pa.Agent.UserProfile.Name,
            pa.Agent.UserProfile.Email,
            pa.Agent.UserProfile.Phone,
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

    

    [HttpPost("createProperty")]
    public async Task<IActionResult> createPropertyAsync([FromBody] Property property)
    {
        _logger.LogInformation($"Property called : {System.Text.Json.JsonSerializer.Serialize(property)}");
        _db.Properties.Add(property);
        await _db.SaveChangesAsync();
        return Ok(property);
    }
    
    // [HttpPost("createUser")]
    // public async Task<IActionResult> CreateUserAsync([FromBody] UserProfile user)
    // {
    //     _logger.LogInformation($"User called : {System.Text.Json.JsonSerializer.Serialize(user)}");
    //     return Ok(await _cosmosService.CreateItemAsync<UserProfile>(user));
    // }

}
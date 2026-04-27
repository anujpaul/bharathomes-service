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
    private readonly CosmosDbService _cosmosService;
    public PropertyController(ImageService imageService, CosmosDbService cosmosService, ILogger<PropertyController> logger)
    {
        _imageService = imageService;
        _cosmosService = cosmosService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Properties()
    {
        _logger.LogInformation("Properties called");
        var properties = await _cosmosService.ReadItemsAsync<Property>();
        return Ok(properties);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPropertyById(string id)
    {
        _logger.LogInformation("here");
        return Ok(await _cosmosService.ReadItemAsync<Property>(id));
    }
    
    [HttpDelete("{id}")]
    public async Task<string> DeletePropertyAsync(string id)
    {
        return await _cosmosService.DeleteItemAsync<Property>(id);
    }

    

    [HttpPost("createProperty")]
    public async Task<IActionResult> createPropertyAsync([FromBody] Property property)
    {
        _logger.LogInformation($"Property called : {System.Text.Json.JsonSerializer.Serialize(property)}");
        return Ok(await _cosmosService.CreateItemAsync<Property>(property));
    }
    
    // [HttpPost("createUser")]
    // public async Task<IActionResult> CreateUserAsync([FromBody] UserProfile user)
    // {
    //     _logger.LogInformation($"User called : {System.Text.Json.JsonSerializer.Serialize(user)}");
    //     return Ok(await _cosmosService.CreateItemAsync<UserProfile>(user));
    // }

}
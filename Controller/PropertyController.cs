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

    [HttpGet("property/{id}")]
    public async Task<IActionResult> GetPropertyById(string id)
    {
        return Ok(await _cosmosService.ReadItemAsync<Property>(id));
    }
    
    [HttpDelete("property/{id}")]
    public async Task<string> DeletePropertyAsync(string id)
    {
        return await _cosmosService.DeleteItemAsync<Property>(id);
    }

    [HttpPost("createagent")]
    public async Task<IActionResult> createAgentAsync([FromBody] Agent agent)
    {
        _logger.LogInformation($"Agent body : {System.Text.Json.JsonSerializer.Serialize(agent)}");
        return Ok(await _cosmosService.CreateItemAsyc<Agent>(agent));
    }

    [HttpPost("createProperty")]
    public async Task<IActionResult> createPropertyAsync([FromBody] Property property)
    {
        _logger.LogInformation($"Property called : {System.Text.Json.JsonSerializer.Serialize(property)}");
        return Ok(await _cosmosService.CreateItemAsyc<Property>(property));
    }
    
    [HttpPost("createUser")]
    public async Task<IActionResult> createUserAsync([FromBody] UserProfile user)
    {
        _logger.LogInformation($"User called : {System.Text.Json.JsonSerializer.Serialize(user)}");
        return Ok(await _cosmosService.CreateItemAsyc<UserProfile>(user));
    }

    [HttpPost("userProfile")]
    [Authorize]
    public async Task<IActionResult> UserProfile()
    {
        foreach (var header in Request.Headers)
        {
            _logger.LogInformation($"Header ===== {header.Key}: {header.Value}");
        }
        var userId    = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? User.FindFirst("sub")?.Value;
        var userName  = User.FindFirst(ClaimTypes.GivenName)?.Value
                        ?? User.FindFirst("given_name")?.Value;
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value
                        ?? User.FindFirst("email")?.Value;

        var iss = User.FindFirst("iss")?.Value;

        var provider =
            iss?.Contains("accounts.google") == true ? "google" :
            iss?.Contains("microsoft") == true ? "microsoft" :
            "other";

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("Missing user identity");
        }

        _logger.LogInformation("UserId: {id}, Name: {name}, Email: {email}, Provider: {provider}", 
            userId, userName, userEmail, provider);

        var profile = await _cosmosService.ReadItemAsync<UserProfile>(userId!);
        if (profile == null)
        {
            profile  = new UserProfile
            {
                Id = userId!,
                Name = userName ?? "Unknown",
                Email = userEmail ?? "Unknown",
                Provider = provider
            };
            await _cosmosService.CreateItemAsyc<UserProfile>(profile);
        }
           
        // profile = await _cosmosService.ReadItemAsync<UserProfile>(userId!);

        return Ok(profile);
    }

    private (string? id, string? name, string? email) GetUserFromHeader()
    {
        var principalHeader = Request.Headers["X-MS-CLIENT-PRINCIPAL"].FirstOrDefault();

        _logger.LogInformation($"MicrosoftPrincipal Header  : {principalHeader}");

        if (string.IsNullOrEmpty(principalHeader))
            return (null, null, null);

        var json = Encoding.UTF8.GetString(Convert.FromBase64String(principalHeader));
        var doc = JsonDocument.Parse(json);

        string? id = null;
        string? name = null;
        string? email = null;

        foreach (var claim in doc.RootElement.GetProperty("claims").EnumerateArray())
        {
            var type = claim.GetProperty("typ").GetString();
            var value = claim.GetProperty("val").GetString();

            if (type == "nameidentifier") id = value;
            if (type == "name") name = value;
            if (type == "emailaddress") email = value;
        }
        return (id, name, email);
    }

}
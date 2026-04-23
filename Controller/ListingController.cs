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
[Route("api")] 

public class ListingController : ControllerBase
{
    private readonly ILogger<ListingController> _logger;
    private readonly ImageService _imageService;
    private readonly CosmosDbService _cosmosService;
    public ListingController(ImageService imageService, CosmosDbService cosmosService, ILogger<ListingController> logger)
    {
        _imageService = imageService;
        _cosmosService = cosmosService;
        _logger = logger;
    }

    [HttpGet("test")]
    public IActionResult Test()
    {
        return Ok("Listing API is active");
    }

    [HttpGet("properties")]
    public async Task<IActionResult> Properties()
    {
        _logger.LogInformation("Properties called");
        

        var properties = await _cosmosService.ReadItemsAsync<Property>();
        return Ok(properties);
    }

    [HttpGet("agents")]
    public async Task<IActionResult> GetAgentsAsync()
    {
        _logger.LogInformation("Agents called");

        var agents = await _cosmosService.ReadItemsAsync<Agent>();

        foreach (var agent in agents)
        {
            if (agent.Id == "a1")
            {
                agent.Image = _imageService.GetImage("Agents", agent.Image);
            }
        }
        return Ok(agents);
    }

    [HttpGet("agent/{id}")] // Use HttpGet for retrieving data
    public async Task<IActionResult> GetAgentAsync(string id)
    {
        _logger.LogInformation("Agent called");
        var agents = await _cosmosService.ReadItemAsync<Agent>(id);
        return Ok(agents);
    }

    [HttpGet("property/{id}")]
    public async Task<IActionResult> GetPropertyById(string id)
    {
        return Ok(await _cosmosService.ReadItemAsync<Property>(id));
    }

    [HttpDelete("agent/{id}")]
    public async Task<string> DeleteAgentAsync(string id)
    {
        return await _cosmosService.DeleteItemAsync<Agent>(id);
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
    // [Authorize]
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

    _logger.LogInformation("UserId: {id}, Name: {name}, Email: {email}", 
        userId, userName, userEmail);
        // var (userId, userName, userEmail) = GetUserFromHeader();

        // // var MicrosoftPrincipalId = Request.Headers["X-MS-CLIENT-PRINCIPAL"].FirstOrDefault();
        // // var MicrosoftPrincipalName = Request.Headers["X-MS-CLIENT-PRINCIPAL-NAME"].FirstOrDefault();
        // // var userId = Request.Headers["X-User-Id"].FirstOrDefault();
        // // var userName = Request.Headers["X-User-Name"].FirstOrDefault();
        // // var userEmail = Request.Headers["X-User-Email"].FirstOrDefault();

        
        // // _logger.LogInformation($"MicrosoftPrincipalName : {MicrosoftPrincipalName}");

        // if (!string.IsNullOrEmpty(userId))
        // {
        //     _logger.LogInformation($"MicrosoftId Id : {userId}");
        //     UserProfile user = new UserProfile
        //                                 {
        //                                     Id = userId,
        //                                     Name = userName ?? "",
        //                                     Email = userEmail ?? ""
        //                                 };
        //     var userProfile = await _cosmosService.ReadItemAsync<UserProfile>(userId);

        //     if (userProfile == null)
        //     {
        //         await _cosmosService.CreateItemAsyc<UserProfile>(user);
        //         _logger.LogInformation("User Created");
        //         return Created("", new { message = "User created" });
        //     }
        // }

        // _logger.LogInformation($"User Id : {userName}");

        



        return Ok(new { message = "User exists" });
        
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
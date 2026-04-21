using System.Data.Common;
using System.Net.Cache;
using System.Threading.Tasks;
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


        
        var MicrodoftId = Request.Headers["X-MS-CLIENT-PRINCIPAL-ID"].FirstOrDefault();
        var userId = Request.Headers["X-User-Id"].FirstOrDefault();
        var userName = Request.Headers["X-User-Name"].FirstOrDefault();
        var userEmail = Request.Headers["X-User-Email"].FirstOrDefault();

        _logger.LogInformation($"MicrodoftId Id : {userId}");

        if (!string.IsNullOrEmpty(userId))
        {
            UserProfile user = new UserProfile
                                        {
                                            Id = userId,
                                            Name = userName,
                                            Email = userEmail
                                        };
            await _cosmosService.CreateItemAsyc<UserProfile>(user);
        }
        
        


        _logger.LogInformation($"User Id : {userName}");

        foreach (var header in Request.Headers)
        {
            _logger.LogInformation($"{header.Key}: {header.Value}");
        }

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


}
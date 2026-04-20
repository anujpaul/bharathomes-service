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

        var userId = Request.Headers["X-MS-CLIENT-PRINCIPAL-ID"].FirstOrDefault();
        var userName = Request.Headers["X-MS-CLIENT-PRINCIPAL-NAME"].FirstOrDefault();

        _logger.LogInformation($"User Id : {userId}");
        _logger.LogInformation($"User Name : {userName}");

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
        return await _cosmosService.DeleteItemAsync<Agent>(id);
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
    
}
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
        System.Console.WriteLine("Properties called");
        

        var properties = await _cosmosService.ReadItemsAsync<Property>("Property");
        return Ok(properties);
    }

    [HttpGet("agents")]
    public async Task<IActionResult> GetAgentsAsync()
    {
        System.Console.WriteLine("Agents called");

        var agents = await _cosmosService.ReadItemsAsync<Agent>("Agent");

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
        System.Console.WriteLine("Agent called");

        var agents = await _cosmosService.ReadItemAsync<Agent>("Agent", id);

        return Ok(agents);
    }

    [HttpDelete("agent/{id}")]
    public async Task<string> DeleteAgentAsync(string id)
    {
        return await _cosmosService.DeleteItemAsync<Agent>("Agent", id);
    }
    
    [HttpDelete("property/{id}")]
    public async Task<string> DeletePropertyAsync(string id)
    {
        return await _cosmosService.DeleteItemAsync<Agent>("Property", id);
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
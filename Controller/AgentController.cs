using Microsoft.AspNetCore.Mvc;
[ApiController]
[Route("api/[controller]")]
public class AgentController : ControllerBase
{
    private readonly CosmosDbService _cosmosService;
    private readonly ILogger<AgentController> _logger;

    private readonly ImageService _imageService;
     public AgentController(CosmosDbService cosmosService, ILogger<AgentController> logger, ImageService imageService)
    {
        _cosmosService = cosmosService;
        _logger = logger;
        _imageService = imageService;

    }
    [HttpGet("agents")]
    public async Task<IActionResult> GetAgentsAsync()
    {
        var agents = await _cosmosService.ReadItemsAsync<Agent>();
        foreach (var agent in agents)
        {
            if (agent.Id == "a1")
            {
                _logger.LogInformation("Pulling image from drive");
                agent.Image = _imageService.GetImage("Agents", agent.Image);
            }
        }
        return Ok(agents);
    }

    [HttpPost("createagent")]
    public async Task<IActionResult> CreateAgentAsync([FromBody] Agent agent)
    {
        _logger.LogInformation($"Agent body : {System.Text.Json.JsonSerializer.Serialize(agent)}");
        return Ok(await _cosmosService.CreateItemAsync<Agent>(agent));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAgentById(string id)
    {
        var agent = await _cosmosService.ReadItemAsync<Agent>(id);
        if (agent == null)
            return NotFound();
        if (agent.Id == "a1")
        {
            _logger.LogInformation("Pulling image from drive");
            agent.Image = _imageService.GetImage("Agents", agent.Image);
        }

        return Ok(agent);
    }

    [HttpDelete("agent/{id}")]
    public async Task<IActionResult> DeleteAgent(string id)
    {
        await _cosmosService.DeleteItemAsync<Agent>(id);
        return Ok();
    }
}
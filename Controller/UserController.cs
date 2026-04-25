using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]

public class UserController : ControllerBase
{
    private readonly CosmosDbService _cosmosService;
    private readonly ILogger<UserController> _logger;
    private readonly ImageService _imageService;

    public UserController(CosmosDbService cosmosService, ILogger<UserController> logger, ImageService imageService)
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
                agent.Image = _imageService.GetImage("Agents", agent.Image);
            }
        }
        return Ok(agents);
    }

    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                  ?? User.FindFirst("sub")?.Value;
        
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        _logger.LogInformation($"User ID from token: {userId}");
        
        var profile = await _cosmosService.ReadItemAsync<UserProfile>(userId);
        if (profile == null)
            return NotFound();

        profile.PasswordHash = null; // Don't return password hash

        return Ok(profile);
    }

    [HttpGet("profile/{id}")]
    public async Task<IActionResult> GetProfileById(string id)
    {
        var profile = await _cosmosService.ReadItemAsync<UserProfile>(id);
        if (profile == null)
            return NotFound();

        return Ok(profile);
    }

    [HttpGet("agent/{id}")]
    public async Task<IActionResult> GetAgentById(string id)
    {
        var agent = await _cosmosService.ReadItemAsync<Agent>(id);
        if (agent == null)
            return NotFound();

        return Ok(agent);
    }

    [HttpDelete("agent/{id}")]
    public async Task<IActionResult> DeleteAgent(string id)
    {
        await _cosmosService.DeleteItemAsync<Agent>(id);
        return Ok();
    }
}
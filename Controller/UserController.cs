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
        var userName  = User.FindFirst(ClaimTypes.Name)?.Value
                        ?? User.FindFirst("name")?.Value;
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value
                        ?? User.FindFirst("email")?.Value;
        var iss = User.FindFirst("iss")?.Value;

        var provider = iss switch
        {
            string s when s.Contains("accounts.google.com") => "google",
            string s when s.Contains("login.microsoftonline.com") => "microsoft",
            _ => "local"
        };
        
        _logger.LogInformation("Name: {name}, Email: {email}, Provider: {provider}, iss : {iss}", 
             userName, userEmail, provider, iss);
        
        userEmail = userEmail?.Trim().ToLower();

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userEmail))
            return Unauthorized();   

        var profile = await _cosmosService.ReadItemByEmailAsync<UserProfile>(userEmail);

        if (provider != "local")
        {
            if (profile == null)
            {
                profile  = new UserProfile
                {
                    Id = userId!,
                    Name = userName ?? "Unknown",
                    Email = userEmail ?? "Unknown",
                    Provider = provider   
                };
                await _cosmosService.CreateItemAsync<UserProfile>(profile);
            }
            else if (profile.Provider == "local")
            {
                profile.Provider = "hybrid";
                await _cosmosService.UpdateItemAsync<UserProfile>(profile);
            }
        }
        
        if (profile == null)
            return NotFound();

        profile.PasswordHash = null; // Don't return password hash

        return Ok(profile);
    }

    [HttpPut("profile")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile([FromBody]UserProfile userProfile)
    { 

        var userEmail = userProfile.Email;
        
        userEmail = userEmail?.Trim().ToLower();

        var profile = await _cosmosService.ReadItemByEmailAsync<UserProfile>(userProfile.Email);

        if (profile == null)
            return NotFound();

        profile.Name = userProfile.Name;
        profile.Email = userProfile.Email;
        profile.Phone = userProfile.Phone;
        _logger.LogInformation($"Phone Number {userProfile.Phone}");
        _logger.LogInformation($"Profile === {profile.Phone}");
        await _cosmosService.UpdateItemAsync<UserProfile>(profile);
            
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
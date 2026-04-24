using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/user")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly CosmosDbService _cosmosService;

    public UserController(CosmosDbService cosmosService)
    {
        _cosmosService = cosmosService;
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                  ?? User.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var profile = await _cosmosService.ReadItemAsync<UserProfile>(userId);
        if (profile == null)
            return NotFound();

        return Ok(profile);
    }
}
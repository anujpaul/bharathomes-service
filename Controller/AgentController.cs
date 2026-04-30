using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class AgentController : ControllerBase
{
    private readonly ILogger<AgentController> _logger;
    private readonly SqlDbContext _db;
    private readonly ImageService _imageService;


    public AgentController(ILogger<AgentController> logger, ImageService imageService, SqlDbContext db)
    {
        _logger = logger;
        _imageService = imageService;
        _db = db;
    }

    [HttpGet("agents")]
    public async Task<IActionResult> GetAgents()
    {
        var agents = await _db.Agents
            .Select(a => new AgentDto
            {
                Id = a.Id,
                Name = a.UserProfile.Name,
                Email = a.UserProfile.Email,
                Phone = a.UserProfile.Phone,
                UserPhoto =a.UserProfile.UserPhoto,
                Rating = a.Rating,
                ListingsCount = a.ListingsCount,
                Specialization = a.Specialization,
                ReraRegistrationNumber = a.ReraRegistrationNumber,
                OperatingLocation = a.OperatingLocation
            })
            .ToListAsync();

        foreach (var agent in agents)
        {
            if (agent.Id == "a1")
            {
                agent.UserPhoto = _imageService.GetImage("Agents",agent.UserPhoto);
            }
        }

        return Ok(agents);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAgentById(string id)
    {
        var agent = await _db.Agents
            .Include(a => a.UserProfile)
            .Where(a => a.Id == id)
            .Select(a => new
            {
                a.Id,
                a.UserProfile.Name,
                a.UserProfile.Email,
                a.UserProfile.Phone,
                a.UserProfile.UserPhoto,
                a.Rating,
                a.ListingsCount,
                a.Specialization,
                a.ReraRegistrationNumber,
                a.OperatingLocation
            })
            .FirstOrDefaultAsync();

        if (agent == null)
            return NotFound(new { message = "Agent not found" });

        return Ok(agent);
    }

    [HttpPost("createagent")]
    public async Task<IActionResult> CreateAgent([FromBody] Agent agent)
    {
        _logger.LogInformation($"Creating agent: {agent.Id}");
        _db.Agents.Add(agent);
        await _db.SaveChangesAsync();
        return Ok(agent);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAgent(string id)
    {
        var agent = await _db.Agents.FindAsync(id);
        if (agent == null)
            return NotFound(new { message = "Agent not found" });

        

        _db.Agents.Remove(agent);
        await _db.SaveChangesAsync();
        return Ok(new { message = "Agent deleted" });
    }
}
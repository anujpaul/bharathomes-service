using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api")] // Changes route to api/listing
public class ListingController : ControllerBase
{
    private readonly ILogger<ListingController> _logger;
    private readonly AppDbContext _context;
    private readonly ImageService _imageService;
    private readonly CosmosDbService _cosmosService;
    public ListingController(ImageService imageService, AppDbContext context, CosmosDbService cosmosService, ILogger<ListingController> logger)
    {
        _imageService = imageService;
        _context = context;
        _cosmosService = cosmosService;
        _logger = logger;
    }

    [HttpGet("test")]
    public IActionResult Test()
    {
        return Ok("Listing API is active");
    }

    [HttpGet("properties")] // Use HttpGet for retrieving data
    public async Task<IActionResult> Properties()
    {
        System.Console.WriteLine("Properties called");
        // var properties = new List<Property>
        //     {
        //         new() {
        //             Id = "1",
        //             Title = "Luxury 4BHK Penthouse",
        //             Price = 45000000,
        //             Location = "Sector 150, Noida Expressway",
        //             City = "Noida",
        //             Beds = 4,
        //             Baths = 4,
        //             Sqft = 3200,
        //             Type = "Apartment",
        //             Image = "https://images.unsplash.com/photo-1512917774080-9991f1c4c750?auto=format&fit=crop&w=800&q=80",
        //             IsFeatured = true,
        //             ExpresswayProximity = true
        //         },
        //         new() {
        //             Id = "2",
        //             Title = "Modern Villa with Taj View",
        //             Price = 28000000,
        //             Location = "Fatehabad Road, Agra",
        //             City = "Agra",
        //             Beds = 3,
        //             Baths = 3,
        //             Sqft = 2400,
        //             Type = "Villa",
        //             Image = "https://images.unsplash.com/photo-1613490493576-7fde63acd811?auto=format&fit=crop&w=800&q=80",
        //             IsFeatured = true,
        //             ExpresswayProximity = false
        //         },
        //         new() {
        //             Id = "3",
        //             Title = "Premium Studio Apartment",
        //             Price = 6500000,
        //             Location = "Knowledge Park III, Greater Noida",
        //             City = "Greater Noida",
        //             Beds = 1,
        //             Baths = 1,
        //             Sqft = 650,
        //             Type = "Apartment",
        //             Image = "https://images.unsplash.com/photo-1522708323590-d24dbb6b0267?auto=format&fit=crop&w=800&q=80",
        //             IsFeatured = false,
        //             ExpresswayProximity = true
        //         },

        //         new() {
        //             Id = "4",
        //             Title = "Premium  Apartment",
        //             Price = 26500000,
        //             Location = "Knowledge Park III, Greater Noida",
        //             City = "Greater Noida",
        //             Beds = 3,
        //             Baths = 2,
        //             Sqft = 1650,
        //             Type = "Apartment",
        //             Image = "https://images.unsplash.com/photo-1522708323590-d24dbb6b0267?auto=format&fit=crop&w=800&q=80",
        //             IsFeatured = false,
        //             ExpresswayProximity = true
        //         }
        // };

        var properties = await _context.Properties.ToListAsync();
        return Ok(properties);
    }

    [HttpGet("agents")] // Use HttpGet for retrieving data
    public async Task<IActionResult> AgentsAsync()
    {
        System.Console.WriteLine("Agent called");
        //  var agents = await _context.Agents.ToListAsync();

        // foreach (var agent in agents)
        // {
        //     if (agent.Id == "a1")
        //     {
        //         agent.Image = _imageService.GetImage("Agents", agent.Image);
        //     }
        // }

        // {
        //     new() {
        //         Id = "a1",
        //         Name = "Sunny Paul",
        //         Role = "Senior Property Consultant",
        //         Image = _imageService.GetImage("Agents","sunny_paul.jpg"),
        //         Rating = 4.9,
        //         ListingsCount = 45,
        //         Specialization = "Apartments, Villas & Plots, Agra and Noida"
        //     },
        //     new() {
        //         Id = "a2",
        //         Name = "Priya Verma",
        //         Role = "Real Estate Advisor",
        //         Image = "https://images.unsplash.com/photo-1573496359142-b8d87734a5a2?auto=format&fit=crop&w=400&q=80",
        //         Rating = 4.8,
        //         ListingsCount = 32,
        //         Specialization = "Luxury Apartments, Villas & Plots, Noida"
        //     },
        //     new() {
        //         Id = "a3",
        //         Name = "Anuj P",
        //         Role = "Principal Investor",
        //         Image = "https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?auto=format&fit=crop&w=400&q=80",
        //         Rating = 4.8,
        //         ListingsCount = 28,
        //         Specialization = "Greater Noida Express Highway"
        //     }
        // };

        var agents = await _cosmosService.ReadItemAsync<Agent>("bharathomes", "Agents", "Agent");

        foreach (var agent in agents)
        {
            if (agent.Id == "a1")
            {
                agent.Image = _imageService.GetImage("Agents", agent.Image);
            }
        }
        return Ok(agents);
    }

    [HttpPost("createagent")]
    public async Task<IActionResult> createAgentAsync([FromBody] Agent agent)
    {
        _logger.LogInformation($"Agent body : {System.Text.Json.JsonSerializer.Serialize(agent)}");
        return Ok(await _cosmosService.CreateItemAsyc<Agent>("bharathomes", "Agents", agent));
    }

    [HttpPost("createProperty")]
    public async Task<IActionResult> createPlantAsync([FromBody] Property property)
    {
        _logger.LogInformation($"Property called : {System.Text.Json.JsonSerializer.Serialize(property)}");
        return Ok(await _cosmosService.CreateItemAsyc<Property>("bharathomes", "Agents", property));
    }
    
}
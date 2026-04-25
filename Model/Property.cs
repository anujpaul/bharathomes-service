using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

public class Property
{
    [Key]
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Location { get; set; } = string.Empty;

    // Matches your TS 'Agra' | 'Noida' | 'Greater Noida'
    public string City { get; set; } = string.Empty;
    public int Beds { get; set; }
    public int Baths { get; set; }
    public int Sqft { get; set; }

    // Matches your TS 'Apartment' | 'Villa' | 'Plot' | 'Commercial'
    public string Type { get; set; } = string.Empty;
    public List<string> Image { get; set; } = new List<string>();
    public bool IsFeatured { get; set; }
    public bool ExpresswayProximity { get; set; }

    public bool IsReraRegistered { get; set; }
    public string? ReraRegistrationNumber { get; set; }
    // Vastu Orientation (North, East, etc.)
    public string? VastuOrientation { get; set; }   
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public List<string>? AgentId { get; set; } 
    public List<string>? Amenities { get; set; }

    [JsonProperty("modeltype")]
    public static string ModelType => "Property";
}
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

public class Property
{
    [Key]
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;

    [JsonProperty("price")]
    public decimal Price { get; set; }

    [JsonProperty("location")]
    public string Location { get; set; } = string.Empty;

    [JsonProperty("city")]
    public string City { get; set; } = string.Empty;

    [JsonProperty("beds")]
    public int Beds { get; set; }

    [JsonProperty("baths")]
    public int Baths { get; set; }

    [JsonProperty("sqft")]
    public int Sqft { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; } = string.Empty;

    [JsonProperty("image")]
    public List<string> Image { get; set; } = new List<string>();

    [JsonProperty("isFeatured")]
    public bool IsFeatured { get; set; }

    [JsonProperty("expresswayProximity")]
    public bool ExpresswayProximity { get; set; }

    [JsonProperty("isReraRegistered")]
    public bool IsReraRegistered { get; set; }

    [JsonProperty("reraRegistrationNumber")]
    public string? ReraRegistrationNumber { get; set; }

    [JsonProperty("vastuOrientation")]
    public string? VastuOrientation { get; set; }

    [JsonProperty("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [JsonProperty("agentId")]
    public List<string>? AgentId { get; set; }

    [JsonProperty("amenities")]
    public List<string>? Amenities { get; set; }

    [JsonProperty("modeltype")]
    public string ModelType { get; set; } = "Property"; // ← also changed to instance property
}
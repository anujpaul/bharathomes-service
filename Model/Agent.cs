using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

public class Agent
{
    [Key]
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    [Required]
    public string UserType { get; set; } = string.Empty; // "agent", "buyer", "seller", "admin"
    public double Rating { get; set; }
    public int ListingsCount { get; set; }
    public string Specialization { get; set; } = string.Empty;
    public string reraRegistrationNumber { get; set; } = string.Empty;
    public string? OperatingLocation { get; set; }
    
    // [JsonProperty("modeltype")]
    public static string ModelType => "Agent";
}
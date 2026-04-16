using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

public class Agent
{
    [Key]

    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public double Rating { get; set; }
    public int ListingsCount { get; set; }
    public string Specialization { get; set; } = string.Empty;
    [JsonProperty("modeltype")]
    public string ModelType { get; set; } = "Agent";
}
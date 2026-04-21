using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

public class UserProfile
{
    [Key]
    [JsonProperty("id")] // Cosmos DB requires 'id'
    public string Id { get; set; } = string.Empty; // This will be your Google/Azure User ID

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("email")]
    public string Email { get; set; } = string.Empty;

    [JsonProperty("modeltype")]
    public static string ModelType => "User"; // Distinguish this from "Agent" in your container
}
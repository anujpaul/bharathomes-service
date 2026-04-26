using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

public class UserProfile
{
    [Key]
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("email")]
    public string Email { get; set; } = string.Empty;

    [JsonProperty("phone")]
    public string Phone { get; set; } = string.Empty;

    [JsonProperty("passwordHash")]
    public string? PasswordHash { get; set; }

    [JsonProperty("provider")]
    public string Provider { get; set; } = string.Empty;

    [JsonProperty("accountStatus")]
    public bool AccountStatus { get; set; }

    [JsonProperty("modeltype")]
    // public string ModelType { get; set; } = "User"; // ← instance property
    public static string ModelType => "User"; 
    public override string ToString()
    {
        return $"Id: {Id}, Name: {Name}, Email: {Email}, Phone: {Phone}, Provider: {Provider}, PasswordHash: {PasswordHash}";
    }
}
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class UserProfile
{
    [Key]
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string UserPhoto { get; set; } = string.Empty;
    public string UserType { get; set; } = string.Empty; // "agent", "buyer", "seller", "admin"
    public string? PasswordHash { get; set; }
    public string Provider { get; set; } = string.Empty;
    public bool AccountStatus { get; set; }
    [JsonIgnore]
    public Agent? Agent { get; set; }

}
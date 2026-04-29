using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Agent
{
    [Key]
    [ForeignKey("UserProfile")]
    public string Id { get; set; } = string.Empty; // same as UserProfile.Id

    public double Rating { get; set; }
    public int ListingsCount { get; set; }
    public string Specialization { get; set; } = string.Empty;
    public string ReraRegistrationNumber { get; set; } = string.Empty;
    public string? OperatingLocation { get; set; }
    // Navigation property back to UserProfile
    public UserProfile UserProfile { get; set; } = null!;
}
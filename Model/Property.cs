using System.ComponentModel.DataAnnotations;

public class Property
{
    [Key]
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Location { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public int Beds { get; set; }
    public int Baths { get; set; }
    public int Sqft { get; set; }
    public string Type { get; set; } = string.Empty;
    public bool IsFeatured { get; set; }
    public bool ExpresswayProximity { get; set; }
    public bool IsReraRegistered { get; set; }
    public string? ReraRegistrationNumber { get; set; }
    public string? VastuOrientation { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    // Navigation properties (replaces List<string>)
    public List<PropertyImage> Images { get; set; } = new();
    public List<PropertyAmenity> Amenities { get; set; } = new();
    public List<PropertyAgent> PropertyAgents { get; set; } = new();
}
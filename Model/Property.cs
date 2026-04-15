using System.ComponentModel.DataAnnotations;

public class Property
{
    [Key]
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
    
    public string Image { get; set; } = string.Empty;
    public bool IsFeatured { get; set; }
    public bool ExpresswayProximity { get; set; }
}
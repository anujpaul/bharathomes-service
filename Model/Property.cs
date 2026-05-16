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
    /// <summary>
    /// Blob URL of the uploaded RERA certificate (PDF or image). Null until
    /// the lister uploads one via POST /api/property/{propertyId}/rera-doc.
    /// </summary>
    public string? ReraDocumentUrl { get; set; }
    public string? VastuOrientation { get; set; }
    /// <summary>
    /// Whether this listing is for sale or for rent. Drives the navbar
    /// Buy/Rent filtering on the listings page.
    /// Values: "sell" | "rent". Defaults to "sell" so existing rows
    /// pre-migration appear under Buy.
    /// </summary>
    public string ListingIntent { get; set; } = "sell";

    /// <summary>
    /// Geocoded coordinates. Populated by GeocodingService when the
    /// property is created. Nullable because (1) the geocoder might
    /// fail or be rate-limited, (2) rows created before this column
    /// existed have no coords, and (3) some property types
    /// (vacant plots without postal addresses) may genuinely not be
    /// geocodable. The map UI renders nothing when either is null.
    /// </summary>
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    public string ListerId { get; set; } = string.Empty;

    public int BuiltYear { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    // Navigation properties (replaces List<string>)
    public List<PropertyImage> Images { get; set; } = new();
    public List<PropertyAmenity> Amenities { get; set; } = new();
    public List<PropertyAgent> PropertyAgents { get; set; } = new();

}


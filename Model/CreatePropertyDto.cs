namespace bharathome_api.Model
{
    public class CreatePropertyDto
    {
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Location { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string? State { get; set; }
        public string? Pincode { get; set; }
        public int Beds { get; set; }
        public int Baths { get; set; }
        public int Sqft { get; set; }
        public string Type { get; set; } = string.Empty;
        public bool IsFeatured { get; set; }
        public bool ExpresswayProximity { get; set; }
        public string ListingIntent { get; set; } = "sell";   // sell | rent
        public List<string> Images { get; set; } = new();
        public List<string> Amenities { get; set; } = new();
        public List<string> AgentId { get; set; } = new();    // matches TS agentId

        public string ListerId { get; set; } = string.Empty;
    }
}

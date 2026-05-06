namespace bharathome_api.DTOs
{
    public class PropertyDetailsDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string Location { get; set; }
        public string City { get; set; }
        public int Beds { get; set; }
        public int Baths { get; set; }
        public int Sqft { get; set; }
        public string Type { get; set; }
        public bool IsFeatured { get; set; }
        public bool ExpresswayProximity { get; set; }
        public bool IsReraRegistered { get; set; }
        public string ReraRegistrationNumber { get; set; }
        public string VastuOrientation { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<string> Images { get; set; }
        public List<string> Amenities { get; set; }
        public List<AgentDto> Agents { get; set; }
    }

}

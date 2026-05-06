namespace bharathome_api.DTOs
{
    public class PanKycDto
    {
        public string PanNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
    public class KycSubmitDto
    {
        public string Role { get; set; } = string.Empty;
        public string Pan { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? ReraNumber { get; set; }
        public string? ReraState { get; set; }
        public string? GstNumber { get; set; }
        public string? CompanyName { get; set; }
    }

    public class KycReviewDto
    {
        public bool Approved { get; set; }
        public string? Reason { get; set; }
    }
}

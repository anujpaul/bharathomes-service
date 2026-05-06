using Microsoft.AspNetCore.Mvc;

namespace bharathome_api.DTOs
{
    public class PanKycDto
    {
        public string PanNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
    public class KycSubmitDto
    {
        [FromForm(Name = "role")] public string Role { get; set; } = string.Empty;
        [FromForm(Name = "pan")] public string Pan { get; set; } = string.Empty;
        [FromForm(Name = "name")] public string Name { get; set; } = string.Empty;
        [FromForm(Name = "reraNumber")] public string? ReraNumber { get; set; }
        [FromForm(Name = "reraState")] public string? ReraState { get; set; }
        [FromForm(Name = "gstNumber")] public string? GstNumber { get; set; }
        [FromForm(Name = "companyName")] public string? CompanyName { get; set; }
    }

    public class KycReviewDto
    {
        public bool Approved { get; set; }
        public string? Reason { get; set; }
    }
}

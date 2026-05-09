namespace bharathome_api.DTOs
{
    public class UserProfileDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string UserPhoto { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty;
        public bool AccountStatus { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? SubscriptionExpiry { get; set; }
        public DateTime? SubscriptionStartedAt { get; set; }
        public string? CurrentPlanCode { get; set; }   // e.g. "pro_yearly"
        public string? CurrentPlanTier { get; set; }   // "basic" | "pro"
        public string Provider { get; set; } = string.Empty;
        public string? KycStatus { get; set; }
    }
}

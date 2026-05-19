using bharathome_api.Model;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class UserProfile
{
    [Key]
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsPaid { get; set; } = false;
    public DateTime? SubscriptionExpiry { get; set; }

    // Active subscription metadata. These are the source of truth for which
    // plan the user is on; PropertyController.GetListingLimit reads CurrentPlanTier
    // to decide whether to apply the Basic (10) or Pro (200) cap.
    // Both stay null while the user is on the free tier.
    public string? CurrentPlanCode { get; set; }      // e.g. "pro_yearly"
    public string? CurrentPlanTier { get; set; }      // "basic" | "pro"
    public DateTime? SubscriptionStartedAt { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string UserPhoto { get; set; } = string.Empty;
    public string UserRole { get; set; } = string.Empty; // "agent", "buyer", "seller", "admin" "paid-seller", "paid-buyer"
    public string? PasswordHash { get; set; }
    public int PropertiesListed { get; set; }
    public string Provider { get; set; } = string.Empty;
    public bool AccountStatus { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? ReraNumber { get; set; }
    [JsonIgnore]
    public Agent? Agent { get; set; }

    public UserStatus UserStatus { get; set; } = UserStatus.Active;
    public string? ReraState
    { get; set; }
    public string? GstNumber { get; set; }
    public string? CompanyName { get; set; }
    public int ResetCount { get; set; } = 0;
    public UserKyc? Kyc { get; set; }

}

public enum UserStatus
{
    Active,
    Inactive,
    Suspended,
    Pending,
}
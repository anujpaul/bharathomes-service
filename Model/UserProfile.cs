using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class UserProfile
{
    [Key]
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsPaid { get; set; } = false;

    public DateTime? SubscriptionExpiry { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string UserPhoto { get; set; } = string.Empty;
    public string UserRole { get; set; } = string.Empty; // "agent", "buyer", "seller", "admin" "paid-seller", "paid-buyer"
    public string? PasswordHash { get; set; }
    public int PropertiesListed { get; set; }
    // KYC
    public KycStatus KycStatus { get; set; } = KycStatus.Pending;
    public string? KycDocumentType { get; set; }
    public string? KycDocumentNumber { get; set; }
    public DateTime? KycSubmittedAt { get; set; }
    public DateTime? KycVerifiedAt { get; set; }
    public string? KycRejectionReason { get; set; }
    public string Provider { get; set; } = string.Empty;
    public bool AccountStatus { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? ReraNumber { get; set; }
    [JsonIgnore]
    public Agent? Agent { get; set; }

    public string? ReraState { get; set; }
    public string? GstNumber { get; set; }
    public string? CompanyName { get; set; }
    public string? KycDocumentUrls { get; set; }  // comma-separated blob URLs

}

public enum KycStatus
{
    Pending,       // just registered, hasn't submitted KYC
    Submitted,     // uploaded docs, waiting for review
    Verified,      // approved
    Rejected       // rejected, can resubmit
}
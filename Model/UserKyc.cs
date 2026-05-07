using System.ComponentModel.DataAnnotations;

namespace bharathome_api.Model
{
    public class UserKyc
    {
        [Key]
        public string UserId { get; set; } = string.Empty;
        public KycStatus KycStatus { get; set; } = KycStatus.Pending;
        public string? KycDocumentType { get; set; }
        public string? KycDocumentNumber { get; set; }
        public DateTime? KycSubmittedAt { get; set; }
        public DateTime? KycVerifiedAt { get; set; }
        public string? KycRejectionReason { get; set; }
        public string? KycDocumentUrls { get; set; }
        public UserProfile UserProfile { get; set; } = null!;
    }

    public enum KycStatus
    {
        Pending,       // just registered, hasn't submitted KYC
        Submitted,     // uploaded docs, waiting for review
        Verified,      // approved
        Rejected       // rejected, can resubmit
    }
}

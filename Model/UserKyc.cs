using System.ComponentModel.DataAnnotations;

namespace bharathome_api.Model
{
    public class UserKyc
    {
        [Key]
        public string UserId { get; set; } = string.Empty;
        public KycStatus Status { get; set; } = KycStatus.Pending;
        public string? DocumentType { get; set; }
        public string? DocumentNumber { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public string? RejectionReason { get; set; }
        public string? DocumentUrls { get; set; }
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

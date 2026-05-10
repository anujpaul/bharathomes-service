using System.ComponentModel.DataAnnotations;

namespace bharathome_api.Model
{
    /// <summary>
    /// A user's request to upgrade to a paid plan, awaiting admin review.
    /// Activated only when an admin approves it (flips UserProfile.IsPaid
    /// and sets SubscriptionExpiry).
    ///
    /// SECURITY NOTE — what we store and what we don't:
    ///   ✓ CardholderName, CardLast4, CardBrand, expiry  (visible to admin
    ///     for review; does not bring us into PCI scope)
    ///   ✗ Full PAN — never persisted. The frontend sends it over HTTPS
    ///     once; we extract last4 + brand and discard the rest.
    ///   ✗ CVV     — never persisted (PCI explicitly forbids storage).
    ///
    /// For real money flows, replace this in-house "store + admin approve"
    /// pattern with a real PSP (Razorpay/Stripe) that returns a token.
    /// </summary>
    public class PaymentRequest
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string UserId { get; set; } = string.Empty;
        public UserProfile? User { get; set; }

        // Plan details snapshotted at submit time so historical records are
        // stable even if the plan catalog price changes later.
        public string PlanCode { get; set; } = string.Empty;     // e.g. "pro_yearly"
        public string PlanTier { get; set; } = string.Empty;     // "basic" | "pro"
        public string PlanCycle { get; set; } = string.Empty;    // "monthly" | "yearly"
        public int    AmountInr { get; set; }

        // Safe card metadata — what an admin needs to recognise the card.
        public string  CardholderName  { get; set; } = string.Empty;
        public string  CardLast4       { get; set; } = string.Empty;   // exactly 4 digits
        public string  CardBrand       { get; set; } = string.Empty;   // "visa" | "mastercard" | "amex" | "rupay" | "discover" | "unknown"
        public int     CardExpiryMonth { get; set; }                   // 1-12
        public int     CardExpiryYear  { get; set; }                   // 4-digit

        public PaymentRequestStatus Status { get; set; } = PaymentRequestStatus.Pending;
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReviewedAt { get; set; }
        public string?   ReviewerEmail { get; set; }
        public string?   RejectionReason { get; set; }
    }

    public enum PaymentRequestStatus
    {
        Pending,
        Approved,
        Rejected
    }
}

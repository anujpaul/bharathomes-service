namespace bharathome_api.DTOs
{
    /// <summary>
    /// Plan codes used by both /api/payment/plans and /api/payment/confirm.
    /// Keep in sync with the Angular upgrade page.
    /// </summary>
    public static class PlanCodes
    {
        public const string BasicMonthly = "basic_monthly";
        public const string BasicYearly  = "basic_yearly";
        public const string ProMonthly   = "pro_monthly";
        public const string ProYearly    = "pro_yearly";
    }

    public class PlanDto
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Tier { get; set; } = string.Empty;          // "basic" | "pro"
        public string Cycle { get; set; } = string.Empty;         // "monthly" | "yearly"
        public int PriceInr { get; set; }
        public int ListingLimit { get; set; }                      // 200 = effectively unlimited
        public bool FeaturedPlacement { get; set; }
        public List<string> Features { get; set; } = new();
    }

    /// <summary>
    /// Submitted by the upgrade page when the user enters card details.
    /// CardNumber is sent over HTTPS once for last4 + brand extraction
    /// and is NEVER persisted. CVV is not part of this DTO — the frontend
    /// asks for it (we still want users to feel like a normal checkout)
    /// but we deliberately don't accept it server-side.
    /// </summary>
    public class ConfirmPaymentRequest
    {
        public string PlanCode { get; set; } = string.Empty;

        public string CardholderName { get; set; } = string.Empty;
        public string CardNumber { get; set; } = string.Empty;     // not stored; only last4 + brand are kept
        public int    CardExpiryMonth { get; set; }
        public int    CardExpiryYear  { get; set; }
    }

    public class PaymentResultDto
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? PlanCode { get; set; }
        public string? RequestId { get; set; }
        public string? Status { get; set; }                         // "pending" | "approved" | "rejected"
        public DateTime? SubscriptionExpiry { get; set; }
    }

    /// <summary>
    /// View row for the admin's payment review queue.
    /// </summary>
    public class PaymentRequestDto
    {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;

        public string PlanCode { get; set; } = string.Empty;
        public string PlanTier { get; set; } = string.Empty;
        public string PlanCycle { get; set; } = string.Empty;
        public int    AmountInr { get; set; }

        public string CardholderName { get; set; } = string.Empty;
        public string CardLast4 { get; set; } = string.Empty;
        public string CardBrand { get; set; } = string.Empty;
        public int    CardExpiryMonth { get; set; }
        public int    CardExpiryYear  { get; set; }

        public string Status { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string? RejectionReason { get; set; }
    }
}

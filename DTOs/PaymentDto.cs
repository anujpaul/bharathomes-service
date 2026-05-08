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

    public class ConfirmPaymentRequest
    {
        public string PlanCode { get; set; } = string.Empty;
    }

    public class PaymentResultDto
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? PlanCode { get; set; }
        public DateTime? SubscriptionExpiry { get; set; }
    }
}

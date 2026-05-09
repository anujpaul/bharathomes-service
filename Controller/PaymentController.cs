using bharathome_api.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace bharathome_api.Controller
{
    /// <summary>
    /// MOCK payment endpoints. No real gateway is called — /confirm just flips
    /// IsPaid + SubscriptionExpiry on the user. Replace the body of Confirm with
    /// real Razorpay/Stripe verification when you're ready to take real payments.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly SqlDbContext _db;

        public PaymentController(SqlDbContext db)
        {
            _db = db;
        }

        // Public: anyone can see the plan catalog.
        [HttpGet("plans")]
        public IActionResult GetPlans()
        {
            return Ok(GetPlanCatalog());
        }

        [HttpPost("confirm")]
        [Authorize]
        public async Task<IActionResult> Confirm([FromBody] ConfirmPaymentRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.PlanCode))
                return BadRequest(new { message = "Plan code is required." });

            var plan = GetPlanCatalog().FirstOrDefault(p => p.Code == req.PlanCode);
            if (plan == null)
                return BadRequest(new { message = "Unknown plan code." });

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var user = await _db.UserProfiles.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return Unauthorized();

            // Extend an existing subscription rather than reset it. If the user
            // is still within their paid window, add the new cycle on top.
            var now = DateTime.UtcNow;
            var hadActiveSub = user.SubscriptionExpiry.HasValue && user.SubscriptionExpiry.Value > now;
            var startFrom = hadActiveSub ? user.SubscriptionExpiry!.Value : now;

            user.IsPaid = true;
            user.SubscriptionExpiry = plan.Cycle == "yearly"
                ? startFrom.AddYears(1)
                : startFrom.AddMonths(1);

            // Persist plan metadata so PropertyController can apply the right
            // listing limit (Basic = 10, Pro = 200) and so the UI can show
            // "You're on Pro Yearly". Keep the original SubscriptionStartedAt
            // when the user is renewing/extending the same tier; refresh it
            // when they switch tiers or buy after a lapsed subscription.
            var switchingTier = user.CurrentPlanTier != plan.Tier;
            if (!hadActiveSub || switchingTier || user.SubscriptionStartedAt == null)
            {
                user.SubscriptionStartedAt = now;
            }
            user.CurrentPlanCode = plan.Code;
            user.CurrentPlanTier = plan.Tier;

            // TODO(payments-history): once we wire up Razorpay/Stripe, also write
            // a row to a Payments table here (planCode, amountInr, paidAt, gatewayId)
            // for receipts, refunds and audit. Tracked separately from this DTO change.

            await _db.SaveChangesAsync();

            return Ok(new PaymentResultDto
            {
                Success = true,
                Message = $"Subscription activated. You're on {plan.Name}.",
                PlanCode = plan.Code,
                SubscriptionExpiry = user.SubscriptionExpiry
            });
        }

        // ── Plan catalog ──────────────────────────────────────────────────
        // Single source of truth. The frontend page mirrors these values for
        // display, but the server is authoritative for what gets activated.
        private static List<PlanDto> GetPlanCatalog() => new()
        {
            new PlanDto
            {
                Code = PlanCodes.BasicMonthly,
                Name = "Basic — Monthly",
                Tier = "basic",
                Cycle = "monthly",
                PriceInr = 299,
                ListingLimit = 10,
                FeaturedPlacement = false,
                Features = new()
                {
                    "Up to 10 active listings",
                    "Standard placement in search",
                    "Email support"
                }
            },
            new PlanDto
            {
                Code = PlanCodes.BasicYearly,
                Name = "Basic — Yearly",
                Tier = "basic",
                Cycle = "yearly",
                PriceInr = 2_990,           // ~17% off vs monthly
                ListingLimit = 10,
                FeaturedPlacement = false,
                Features = new()
                {
                    "Up to 10 active listings",
                    "Standard placement in search",
                    "Email support",
                    "Save ₹598 vs monthly"
                }
            },
            new PlanDto
            {
                Code = PlanCodes.ProMonthly,
                Name = "Pro — Monthly",
                Tier = "pro",
                Cycle = "monthly",
                PriceInr = 999,
                ListingLimit = 200,         // matches PropertyController.GetListingLimit
                FeaturedPlacement = true,
                Features = new()
                {
                    "Unlimited active listings",
                    "Featured placement on home page",
                    "Priority WhatsApp support",
                    "Lead analytics"
                }
            },
            new PlanDto
            {
                Code = PlanCodes.ProYearly,
                Name = "Pro — Yearly",
                Tier = "pro",
                Cycle = "yearly",
                PriceInr = 9_990,           // ~17% off vs monthly
                ListingLimit = 200,
                FeaturedPlacement = true,
                Features = new()
                {
                    "Unlimited active listings",
                    "Featured placement on home page",
                    "Priority WhatsApp support",
                    "Lead analytics",
                    "Save ₹1,998 vs monthly"
                }
            },
        };
    }
}

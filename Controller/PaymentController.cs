using bharathome_api.DTOs;
using bharathome_api.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace bharathome_api.Controller
{
    /// <summary>
    /// Plan upgrade endpoints. The flow is:
    ///   1. POST /api/payment/confirm with plan + card details
    ///         → creates a PaymentRequest with status=Pending
    ///         → DOES NOT activate the plan
    ///   2. Admin reviews the request in the admin app and either
    ///      approves (which then flips IsPaid + sets SubscriptionExpiry)
    ///      or rejects with a reason.
    ///
    /// IMPORTANT: this is a mock. No real charge happens. The card number
    /// is sent over HTTPS once for last4/brand extraction and is NEVER
    /// persisted. CVV is not accepted at all (PCI forbids storage).
    /// Replace with Razorpay/Stripe before taking real money.
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

            // ── Card validation ────────────────────────────────────────────
            //
            // We never store the full PAN; this is just a sanity gate so we
            // don't queue obviously-bogus requests for the admin to wade
            // through. Real charge validation happens at the gateway later.
            if (string.IsNullOrWhiteSpace(req.CardholderName))
                return BadRequest(new { code = "CARD_NAME", message = "Cardholder name is required." });

            var digits = new string((req.CardNumber ?? "").Where(char.IsDigit).ToArray());
            if (digits.Length < 12 || digits.Length > 19)
                return BadRequest(new { code = "CARD_NUMBER", message = "Card number length looks wrong." });
            if (!PassesLuhn(digits))
                return BadRequest(new { code = "CARD_LUHN", message = "Card number didn't pass the checksum." });

            if (req.CardExpiryMonth < 1 || req.CardExpiryMonth > 12)
                return BadRequest(new { code = "CARD_EXPIRY", message = "Expiry month must be 1–12." });
            // Expiry must be the end of the stated month or later.
            var now = DateTime.UtcNow;
            var expiryEnd = new DateTime(req.CardExpiryYear, req.CardExpiryMonth, 1)
                .AddMonths(1).AddDays(-1);
            if (expiryEnd < now.Date)
                return BadRequest(new { code = "CARD_EXPIRY", message = "Card has expired." });

            var brand = DetectBrand(digits);
            var last4 = digits[^4..];

            // ── Create the pending request ─────────────────────────────────
            //
            // Snapshot the plan price/cycle/tier on the request itself so
            // historical records survive future catalog edits.
            var pending = new PaymentRequest
            {
                UserId          = userId,
                PlanCode        = plan.Code,
                PlanTier        = plan.Tier,
                PlanCycle       = plan.Cycle,
                AmountInr       = plan.PriceInr,
                CardholderName  = req.CardholderName.Trim(),
                CardLast4       = last4,
                CardBrand       = brand,
                CardExpiryMonth = req.CardExpiryMonth,
                CardExpiryYear  = req.CardExpiryYear,
                Status          = PaymentRequestStatus.Pending,
                SubmittedAt     = now,
            };
            _db.PaymentRequests.Add(pending);
            await _db.SaveChangesAsync();

            return Ok(new PaymentResultDto
            {
                Success = true,
                Message = $"Payment received for {plan.Name}. An admin will activate your subscription within 24 hours.",
                PlanCode = plan.Code,
                RequestId = pending.Id,
                Status = "pending"
            });
        }

        // ── Helpers ──────────────────────────────────────────────────────

        /// <summary>Standard Luhn (mod-10) checksum used to spot typos in card numbers.</summary>
        private static bool PassesLuhn(string digits)
        {
            var sum = 0;
            var alt = false;
            for (var i = digits.Length - 1; i >= 0; i--)
            {
                var d = digits[i] - '0';
                if (alt)
                {
                    d *= 2;
                    if (d > 9) d -= 9;
                }
                sum += d;
                alt = !alt;
            }
            return sum % 10 == 0;
        }

        /// <summary>
        /// BIN-based brand detection. Mirrors what the frontend shows live,
        /// but we re-derive on the server because the client is not trusted.
        /// </summary>
        private static string DetectBrand(string digits)
        {
            if (digits.Length == 0) return "unknown";
            // Visa: starts with 4
            if (digits[0] == '4') return "visa";
            // Amex: 34 or 37
            if (digits.Length >= 2)
            {
                var p2 = int.Parse(digits.Substring(0, 2));
                if (p2 == 34 || p2 == 37) return "amex";
                // Mastercard: 51-55, or 2221-2720 (handled below)
                if (p2 >= 51 && p2 <= 55) return "mastercard";
                // Diners: 36, 38, 30
                if (p2 == 36 || p2 == 38 || p2 == 30) return "diners";
                // Discover: 65 (also 6011 below)
                if (p2 == 65) return "discover";
                // RuPay: 60, 65 (overlaps Discover — Discover wins above), 81, 82
                if (p2 == 60 || p2 == 81 || p2 == 82) return "rupay";
            }
            if (digits.Length >= 4)
            {
                var p4 = int.Parse(digits.Substring(0, 4));
                if (p4 >= 2221 && p4 <= 2720) return "mastercard";
                if (p4 == 6011) return "discover";
            }
            return "unknown";
        }

        // ── Plan catalog ──────────────────────────────────────────────────
        // Single source of truth. Internal helper now used both by GET /plans
        // and by Confirm's snapshot-into-PaymentRequest path. Also exposed to
        // the admin app via PaymentReviewService when activating an approved
        // request (so the admin app doesn't have to mirror the catalog).
        public static List<PlanDto> GetPlanCatalog() => new()
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

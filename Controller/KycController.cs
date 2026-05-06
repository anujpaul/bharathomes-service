using bharathome_api.DTOs;
using bharathome_api.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace bharathome_api.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class KycController : ControllerBase
    {
        private readonly KycService _kycService;
        private readonly SqlDbContext _db;
        public KycController(SqlDbContext db, KycService kycService)
        {
            _db = db;
            _kycService = kycService;
        }
        [HttpPost("kyc/verify-pan")]
        [Authorize]
        public async Task<IActionResult> VerifyPan([FromBody] PanKycDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _db.UserProfiles.FindAsync(userId);

            var result = await _kycService.VerifyPanAsync(dto.PanNumber, user.Name);

            if (!result.IsValid)
                return BadRequest(new { code = "INVALID_PAN", message = "PAN number is invalid." });

            if (!result.NameMatch)
                return BadRequest(new
                {
                    code = "NAME_MISMATCH",
                    message = "Name on PAN does not match your profile name."
                });

            user.KycStatus = KycStatus.Verified;
            user.KycVerifiedAt = DateTime.UtcNow;
            user.KycDocumentType = "PAN";
            user.KycDocumentNumber = dto.PanNumber; // store masked: ABCDE1234F → ABCDE***4F
            await _db.SaveChangesAsync();

            return Ok(new { message = "KYC verified successfully." });
        }

        [HttpPost("submit")]
        public async Task<IActionResult> Submit([FromBody] KycSubmitDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _db.UserProfiles.FindAsync(userId);
            if (user == null) return Unauthorized();

            user.UserRole = dto.Role;
            user.KycDocumentNumber = dto.Pan;  //MaskPan(dto.Pan);
            user.ReraNumber = dto.ReraNumber;
            //user.ReraState = dto.ReraState;
            //user.GstNumber = dto.GstNumber;
            //user.CompanyName = dto.CompanyName;
            user.KycStatus = KycStatus.Submitted;  // ← admin reviews manually
            user.KycSubmittedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            // TODO: send email to admin to review
            return Ok(new { message = "KYC submitted. We'll verify within 1-2 business days." });
        }

        private string MaskPan(string pan) =>
            pan.Length == 10 ? pan[..5] + "****" + pan[9] : pan;

        [HttpGet("status")]
        public async Task<IActionResult> GetStatus()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _db.UserProfiles.FindAsync(userId);
            if (user == null) return Unauthorized();

            return Ok(new
            {
                status = user.KycStatus.ToString().ToLower(),
                email = user.Email,
                rejectionReason = user.KycRejectionReason
            });
        }
    }
}

using bharathome_api.DTOs;
using bharathome_api.Model;
using bharathome_api.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace bharathome_api.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class KycController : ControllerBase
    {
        private readonly KycService _kycService;
        private readonly SqlDbContext _db;
        private readonly ImageService _imageService;


        public KycController(SqlDbContext db, KycService kycService, ImageService imageService)
        {
            _db = db;
            _kycService = kycService;
            _imageService = imageService;
        }
        [HttpPost("verify-pan")]
        [Authorize]
        public async Task<IActionResult> VerifyPan([FromBody] PanKycDto dto)
        {
            Console.WriteLine($"Name is {dto.Name}, Pan: {dto.PanNumber}");
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _db.UserProfiles
                .Include(u => u.Kyc)
                .FirstOrDefaultAsync(u => u.Id == userId);

            var result = await _kycService.VerifyPanAsync(dto.PanNumber, user.Name);

            if (!result.IsValid)
                return BadRequest(new { code = "INVALID_PAN", message = "PAN number is invalid." });

            if (!result.NameMatch)
                return BadRequest(new
                {
                    code = "NAME_MISMATCH",
                    message = "Name on PAN does not match your profile name."
                });

            if (user.Kyc == null)
            {
                user.Kyc = new UserKyc { UserId = user.Id };
            }

            user.Kyc.Status = KycStatus.Verified;
            user.Kyc.VerifiedAt = DateTime.UtcNow;
            user.Kyc.DocumentType = "PAN";
            user.Kyc.DocumentNumber = dto.PanNumber; // store masked: ABCDE1234F → ABCDE***4F
            await _db.SaveChangesAsync();

            return Ok(new { message = "KYC verified successfully." });
        }

        [HttpPost("submit")]
        [Authorize]
        public async Task<IActionResult> Submit([FromForm] KycSubmitDto dto, [FromForm] List<IFormFile> documents)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            // IMPORTANT: include Kyc — FindAsync does NOT load navigation properties,
            // and the rest of this method writes to user.Kyc.*
            var user = await _db.UserProfiles
                .Include(u => u.Kyc)
                .FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return Unauthorized();

            // Validate basic required text fields up-front so we don't waste time
            // uploading files for a request that will fail anyway.
            if (string.IsNullOrWhiteSpace(dto.Role))
                return BadRequest(new { message = "Role is required." });
            if (string.IsNullOrWhiteSpace(dto.Pan))
                return BadRequest(new { message = "PAN is required." });

            var role = dto.Role.ToLowerInvariant();
            if (role is "agent" or "builder" or "developer" && string.IsNullOrWhiteSpace(dto.ReraNumber))
                return BadRequest(new { message = "RERA number is required for this role." });
            if (role is "builder" or "developer")
            {
                if (string.IsNullOrWhiteSpace(dto.GstNumber))
                    return BadRequest(new { message = "GST number is required for this role." });
                if (string.IsNullOrWhiteSpace(dto.CompanyName))
                    return BadRequest(new { message = "Company name is required for this role." });
            }
            if (role != "owner" && (documents == null || documents.Count == 0))
                return BadRequest(new { message = "Please upload at least one supporting document." });

            // Upload each document to blob storage
            var docUrls = new List<string>();
            if (documents != null)
            {
                foreach (var doc in documents)
                {
                    var allowedTypes = new[] { "application/pdf", "image/jpeg", "image/jpg", "image/png" };
                    if (!allowedTypes.Contains(doc.ContentType))
                        return BadRequest(new { message = $"File {doc.FileName} is not allowed. Use PDF, JPG or PNG." });

                    if (doc.Length > 5 * 1024 * 1024)
                        return BadRequest(new { message = $"File {doc.FileName} exceeds 5MB limit." });

                    using var stream = doc.OpenReadStream();
                    var url = await _imageService.UploadImageAsync(
                        stream, $"kyc/{userId}", doc.FileName, doc.ContentType);
                    docUrls.Add(url);
                }
            }

            // Make sure a Kyc row exists. Users may submit before ever calling /verify-pan.
            if (user.Kyc == null)
            {
                user.Kyc = new UserKyc { UserId = user.Id };
                _db.UserKycs.Add(user.Kyc);
            }

            user.UserRole = dto.Role;
            user.Kyc.DocumentNumber = MaskPan(dto.Pan);
            user.ReraNumber = dto.ReraNumber;
            user.ReraState = dto.ReraState;
            user.GstNumber = dto.GstNumber;
            user.CompanyName = dto.CompanyName;
            // Preserve previously uploaded docs if no new ones came through
            user.Kyc.DocumentUrls = docUrls.Count > 0
                ? string.Join(",", docUrls)
                : user.Kyc.DocumentUrls;
            user.Kyc.Status = KycStatus.Submitted;
            user.Kyc.SubmittedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return Ok(new { message = "KYC submitted. We'll verify within 1-2 business days." });
        }

        private string MaskPan(string pan) =>
            pan.Length == 10 ? pan[..5] + "****" + pan[9] : pan;

        [HttpGet("status")]
        [Authorize]
        public async Task<IActionResult> GetStatus()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var kyc = await _db.UserKycs
                    .FirstOrDefaultAsync(k => k.UserId == userId);

            // No KYC row yet just means the user hasn't started — return "pending",
            // not 401, otherwise the frontend redirects them out of the KYC page.
            if (kyc == null)
            {
                return Ok(new
                {
                    status = "pending",
                    rejectionReason = (string?)null
                });
            }

            return Ok(new
            {
                status = kyc.Status.ToString().ToLower(),
                rejectionReason = kyc.RejectionReason
            });
        }
    }
}

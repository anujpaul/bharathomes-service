using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static UserService;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserService _userService;
    private readonly TokenService _tokenService;

    private readonly OtpService _otpService;
    private readonly SqlDbContext _db;

    public AuthController(UserService userService, TokenService tokenService, SqlDbContext db, OtpService otpService)
    {
        _userService = userService;
        _tokenService = tokenService;
        _db = db;
        _otpService = otpService;
    }

    public record LoginRequest(string Email, string Password);
    public record RegisterRequest(string Email, string Password, string Name);

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userService.ValidateCredentials(request.Email, request.Password);

        if (user == null)
            return Unauthorized(new { message = "Invalid email or password" });

        var access_token = _tokenService.GenerateJwt(user);

        var responseList = new List<AuthResponse>
        {
            new AuthResponse 
            { 
                access_token = access_token,
                expires_on = DateTime.UtcNow.AddHours(1).ToString("o"),
                // You must use 'new UserClaim' for each item in the list
                user_claims = new List<UserClaim>
                {
                    new UserClaim { typ = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname", val = user.Name },
                    new UserClaim { typ = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", val = user.Id },
                    new UserClaim { typ = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", val = user.Email }
                },
                user_id = user.Email
            }
        };

        // Wrap the list in Ok() to return it as an IActionResult
        return Ok(responseList);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _userService.CreateLocalUser(request.Email, request.Password, request.Name);

        return result.Status switch
        {
            RegisterStatus.EmailTaken => Conflict(new { message = "Email already registered" }),

            // Tell the frontend to show the OTP step — do NOT send OTP yet (avoid spam)
            RegisterStatus.RequiresOtp => Ok(new { requiresOtp = true }),

            RegisterStatus.Created => Ok(BuildAuthResponse(result.User!)),
            _ => StatusCode(500, new { message = "Unexpected error" })
        };
    }

    [HttpPost("send-merge-otp")]
    [AllowAnonymous]
    public async Task<IActionResult> SendMergeOtp([FromBody] SendOtpRequest request)
    {
        // Don't reveal whether the account exists — always respond OK
        var exists = await _db.UserProfiles
            .AnyAsync(u => u.Email == request.Email && u.Provider != "local");

        if (exists)
            await _otpService.SendOtpAsync(request.Email);

        return Ok(new { message = "If a matching account exists, an OTP has been sent." });
    }

    [HttpPost("verify-merge")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyMerge([FromBody] VerifyMergeRequest request)
    {
        if (!_otpService.ValidateOtp(request.Email, request.Otp))
            return Unauthorized(new { message = "Invalid or expired OTP" });

        var user = await _userService.MergeAfterOtp(request.Email, request.Password, request.Name);
        if (user == null)
            return BadRequest(new { message = "Account merge failed" });

        return Ok(BuildAuthResponse(user));
    }

    // ── records & helpers ──────────────────────────────────────────────────────

    public record SendOtpRequest(string Email);
    public record VerifyMergeRequest(string Email, string Otp, string Password, string Name);

    private List<AuthResponse> BuildAuthResponse(UserProfile user)
    {
        var token = _tokenService.GenerateJwt(user);
        return new List<AuthResponse>
        {
            new()
            {
                access_token = token,
                expires_on = DateTime.UtcNow.AddHours(1).ToString("o"),
                user_id = user.Email,
                user_claims = new()
                {
                    new() { typ = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname",         val = user.Name },
                    new() { typ = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",    val = user.Id },
                    new() { typ = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress",      val = user.Email }
                }
            }
        };
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody]ForgotPasswordRequest request)
    {
        try
        {
            //await _userService.InitiatePasswordReset(request.Email);
            return Ok(new {message = "If an account exists, a reset email has been sent"});
        }
        catch(Exception ex)
        {
            return StatusCode(500, new {message = ex.Message});
        }
    }

    [HttpPost("send-reset-otp")]
    [AllowAnonymous]
    public async Task<IActionResult> SendResetOtp([FromBody] SendOtpRequest request)
    {
        var exists = await _db.UserProfiles.AnyAsync(u => u.Email == request.Email);
        if (exists)
            await _otpService.SendOtpAsync(request.Email);

        // Always return OK — don't reveal if account exists
        return Ok(new { message = "If an account exists, a reset code has been sent." });
    }

    [HttpPost("verify-reset")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyReset([FromBody] VerifyResetRequest request)
    {
        if (!_otpService.ValidateOtp(request.Email, request.Otp))
            return Unauthorized(new { message = "Invalid or expired code" });

        if (request.NewPassword.Length < 8)
            return BadRequest(new { message = "Password must be at least 8 characters" });

        var success = await _userService.ResetPasswordWithOtp(request.Email, request.NewPassword);
        if (!success)
            return BadRequest(new { message = "Account not found" });

        return Ok(new { message = "Password reset successfully" });
    }

    public record VerifyResetRequest(string Email, string Otp, string NewPassword);
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserService _userService;
    private readonly TokenService _tokenService;

    public AuthController(UserService userService, TokenService tokenService)
    {
        _userService = userService;
        _tokenService = tokenService;
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
        System.Console.WriteLine($"Email {request.Email}, Password : {request.Password}");
        try
        {
            var user = await _userService.CreateLocalUser(
                request.Email, request.Password, request.Name);
            
            if (user == null)
                return Conflict(new { message = "Email already registered" });

            var token = _tokenService.GenerateJwt(user);

            var responseList = new List<AuthResponse>
            {
                new AuthResponse 
                { 
                    access_token = token,
                    expires_on = DateTime.UtcNow.AddHours(1).ToString("o"),
                    // You must use 'new UserClaim' for each item in the list
                    user_claims = new List<UserClaim>
                    {
                        new UserClaim { typ = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", val = user.Id },
                        new UserClaim { typ = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname", val = user.Name },
                        new UserClaim { typ = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", val = user.Email }
                    },
                    user_id = user.Email
                }
            };

        // Wrap the list in Ok() to return it as an IActionResult
        return Ok(responseList);

        }
        catch (Exception ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody]ForgotPasswordRequest request)
    {
        try
        {
            await _userService.InitiatePasswordReset(request.Email);
            return Ok(new {message = "If an account exists, a reset email has been sent"});
        }
        catch(Exception ex)
        {
            return StatusCode(500, new {message = ex.Message});
        }
    }
}
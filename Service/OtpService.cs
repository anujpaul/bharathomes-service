using Microsoft.Extensions.Caching.Memory;

public class OtpService
{
    private readonly IMemoryCache _cache;
    private readonly IEmailService _email;

    public OtpService(IMemoryCache cache, IEmailService email)
    {
        _cache = cache;
        _email = email;
    }


    
    public async Task SendOtpAsync(string toEmail)
    {
        var otp = Random.Shared.Next(100000, 999999).ToString();
        _cache.Set($"reset_otp:{toEmail}", otp, TimeSpan.FromMinutes(10));
        await _email.SendAsync(toEmail, "Reset your password — BharatHomes",
            $"Your password reset code is <b>{otp}</b>. It expires in 10 minutes.");
    }

    public bool ValidateOtp(string email, string otp)
    {
        var key = $"reset_otp:{email}";
        if (_cache.TryGetValue(key, out string? stored) && stored == otp)
        {
            _cache.Remove(key);
            return true;
        }
        return false;
    }
}
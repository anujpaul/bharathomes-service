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

    public async Task SendMergeOtpAsync(string toEmail)
    {
        var otp = Random.Shared.Next(100000, 999999).ToString();
        _cache.Set($"merge_otp:{toEmail}", otp, TimeSpan.FromMinutes(10));
        await _email.SendAsync(toEmail, "Verify your identity — BharatHomes",
            $"Your one-time code is <b>{otp}</b>. It expires in 10 minutes.");
    }

    public bool ValidateOtp(string email, string otp)
    {
        var key = $"merge_otp:{email}";
        if (_cache.TryGetValue(key, out string? stored) && stored == otp)
        {
            _cache.Remove(key);
            return true;
        }
        return false;
    }
}
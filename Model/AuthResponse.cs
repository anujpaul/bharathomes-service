public class AuthResponse
{
    public string access_token { get; set; } = string.Empty;
    public string expires_on { get; set; } = string.Empty;
    public string id_token { get; set; } = string.Empty;
    public string provider_name { get; set; } = string.Empty;
    public List<UserClaim> user_claims { get; set; } = new();
    public string user_id { get; set; } = string.Empty;
}
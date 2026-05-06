using bharathome_api.Model;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using static System.Net.WebRequestMethods;

namespace bharathome_api.Service
{
    public class KycService
    {
        private readonly HttpClient _http;
        private readonly string _token;

        public KycService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _token = config["Surepass:Token"];
        }
        public async Task<PanVerificationResult> VerifyPanAsync(string panNumber, string name)
        {
            var request = new HttpRequestMessage(HttpMethod.Post,
                "https://kyc-api.surepass.io/api/v1/pan/pan");

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", _token);

            request.Content = JsonContent.Create(new { id_number = panNumber });

            var response = await _http.SendAsync(request);
            var body = await response.Content.ReadFromJsonAsync<SurepassPanResponse>();

            return new PanVerificationResult
            {
                IsValid = body?.Data?.Valid ?? false,
                NameOnPan = body?.Data?.Name ?? "",
                NameMatch = body?.Data?.Name
                    .Contains(name, StringComparison.OrdinalIgnoreCase) ?? false
            };
        }
    }
}

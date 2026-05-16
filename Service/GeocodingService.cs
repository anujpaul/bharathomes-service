using System.Text.Json;

namespace bharathome_api.Service;

public interface IGeocodingService
{
    Task<(double lat, double lon)?> GeocodeAsync(
        string? location, string? city, string? country = "India",
        CancellationToken ct = default);
}

/// <summary>
/// Free geocoder backed by OpenStreetMap Nominatim. No API key required.
///
/// Nominatim policy (https://operations.osmfoundation.org/policies/nominatim/):
///   - Max 1 request per second
///   - MUST send a meaningful User-Agent identifying the app + a contact
///   - Cache results aggressively (we store coords on the property row so
///     a given listing is only geocoded once)
///
/// This service never throws on geocode failure — returns null and logs.
/// The caller should treat null as "leave latitude/longitude null and
/// move on" so a flaky geocoder doesn't break property creation.
/// </summary>
public class GeocodingService : IGeocodingService
{
    private readonly HttpClient _http;
    private readonly ILogger<GeocodingService> _logger;

    public GeocodingService(HttpClient http, ILogger<GeocodingService> logger)
    {
        _http = http;
        _logger = logger;

        // Nominatim REQUIRES this. Without it they return 403.
        // Replace the email with whatever you'd want them to contact if
        // your traffic ever becomes problematic.
        if (!_http.DefaultRequestHeaders.UserAgent.Any())
        {
            _http.DefaultRequestHeaders.UserAgent.ParseAdd(
                "BharatHomes/1.0 (contact: paul.anuj@gmail.com)");
        }
    }

    public async Task<(double lat, double lon)?> GeocodeAsync(
        string? location, string? city, string? country = "India",
        CancellationToken ct = default)
    {
        // Build the freeform query. Joining with commas matches how
        // Nominatim's freeform parser expects address components.
        var parts = new[] { location, city, country }
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .ToArray();

        if (parts.Length == 0)
        {
            _logger.LogWarning("Geocode skipped — no location/city provided");
            return null;
        }

        var query = string.Join(", ", parts);
        var url = "https://nominatim.openstreetmap.org/search"
                + $"?q={Uri.EscapeDataString(query)}"
                + "&format=json&limit=1";

        try
        {
            using var resp = await _http.GetAsync(url, ct);
            if (!resp.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Nominatim returned {Status} for query '{Query}'",
                    resp.StatusCode, query);
                return null;
            }

            var json = await resp.Content.ReadAsStringAsync(ct);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            // No matches — empty array.
            if (root.ValueKind != JsonValueKind.Array || root.GetArrayLength() == 0)
            {
                _logger.LogInformation(
                    "Nominatim: no match for '{Query}'", query);
                return null;
            }

            var first = root[0];
            // Nominatim returns lat/lon as strings — yes, strings.
            if (!first.TryGetProperty("lat", out var latProp) ||
                !first.TryGetProperty("lon", out var lonProp))
            {
                return null;
            }

            if (!double.TryParse(latProp.GetString(), out var lat) ||
                !double.TryParse(lonProp.GetString(), out var lon))
            {
                return null;
            }

            _logger.LogInformation(
                "Geocoded '{Query}' -> ({Lat}, {Lon})", query, lat, lon);
            return (lat, lon);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Geocode failed for '{Query}'", query);
            return null;
        }
    }
}

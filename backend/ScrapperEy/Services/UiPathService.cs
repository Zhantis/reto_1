using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

namespace ScrapperEy.Services
{
    public class UiPathService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public UiPathService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            var content = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("grant_type", "refresh_token"),
            new KeyValuePair<string, string>("client_id", _config["UiPath:ClientId"]),
            new KeyValuePair<string, string>("refresh_token", _config["UiPath:RefreshToken"])
        });

            var response = await _httpClient.PostAsync("identity_/connect/token", content);
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(responseString);
            return doc.RootElement.GetProperty("access_token").GetString();
        }

        public async Task<string> StartJobAsync()
        {
            string accessToken = await GetAccessTokenAsync();

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var payload = new
            {
                startInfo = new
                {
                    ReleaseKey = _config["UiPath:ReleaseKey"],
                    Strategy = "All",  // Or "Specific"
                    RobotIds = new int[] { },  // Optional
                    NoOfRobots = 0,
                    Source = "Manual"
                }
            };

            var jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            string fullUrl = _config["UiPath:OrchestratorUrl"] + "odata/Jobs/UiPath.Server.Configuration.OData.StartJobs";

            var response = await _httpClient.PostAsync(fullUrl, content);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}

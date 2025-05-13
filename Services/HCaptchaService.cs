using System.Text.Json;
using VoteWiselyBackend.Contracts;

namespace VoteWiselyBackend.Services
{
    public class HCaptchaService
    {
        private readonly HttpClient _httpClient;
        private readonly string _secretKey;
        private readonly IConfiguration _configuration;
        public HCaptchaService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient();
            _secretKey = _configuration["HCaptch:SecretKey"]
                ?? Environment.GetEnvironmentVariable("HCAPTCHA_SECRET_KEY") ?? throw new ArgumentNullException("HCAPTCHA_SECRET_KEY");
        }

        public async Task<HCaptchaVerificationResponse?> VerifyHCaptchaAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return new HCaptchaVerificationResponse { Success = false, ErrorCodes = new List<string> { "missing-input-response" } };
            }

            var data = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "secret", _secretKey },
                { "response", token },
            });

            try
            {

                var response = await _httpClient.PostAsync("https://api.hcaptcha.com/siteverify", data);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var verificationResponse = JsonSerializer.Deserialize<HCaptchaVerificationResponse>(jsonResponse, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (verificationResponse == null)
                {
                    return new HCaptchaVerificationResponse { Success = false, ErrorCodes = new List<string> { "deserialization-failed" } };
                }

                return await response.Content.ReadFromJsonAsync<HCaptchaVerificationResponse>();
            }
            catch (HttpRequestException ex)
            {
                return new HCaptchaVerificationResponse { Success = false, ErrorCodes = new List<string> { "http-error" } };
            }
            catch (Exception ex)
            {
                return new HCaptchaVerificationResponse { Success = false, ErrorCodes = new List<string> { "verification-error" } };
            }
        }
    }
}

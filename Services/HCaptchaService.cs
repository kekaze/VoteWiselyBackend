using System.Text.Json;
using VoteWiselyBackend.Contracts;

namespace VoteWiselyBackend.Services
{
    public class HCaptchaService(IHttpClientFactory httpClientFactory, string secretKey)
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient();

        public async Task<HCaptchaVerificationResponse?> VerifyHCaptchaAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return new HCaptchaVerificationResponse { Success = false, ErrorCodes = new List<string> { "missing-input-response" } };
            }

            var data = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "secret", secretKey },
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

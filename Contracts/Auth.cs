using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace VoteWiselyBackend.Contracts
{
    public class SignUpRequest
    {
        [JsonPropertyName("full_name")]
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }

        [JsonPropertyName("confirm_password")]
        public required string ConfirmPassword { get; set; }

        [Required]
        [JsonPropertyName("captcha_token")]
        public string CaptchaToken { get; set; } = string.Empty;
    }

    public class LoginRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public class GoTrueExMessage
    {
        public string? Msg { get; set; }
    }

    public class HCaptchaVerificationResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }
        [JsonPropertyName("challenge_ts")]
        public DateTime ChallengeTs { get; set; }
        [JsonPropertyName("hostname")]
        public string Hostname { get; set; } = string.Empty;
        [JsonPropertyName("error-codes")]
        public List<string> ErrorCodes { get; set; } = new List<string>();
    }

    public class HCaptchaVerificationRequest
    {
        public string Secret { get; set; } = string.Empty;
        public string Response { get; set; } = string.Empty;
    }
}

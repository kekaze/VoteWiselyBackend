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

    public class OAuthSignInRequest
    {
        
    }
}

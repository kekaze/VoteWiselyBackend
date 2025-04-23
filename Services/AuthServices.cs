using Supabase.Gotrue;
using System.Text.RegularExpressions;
using VoteWiselyBackend.Contracts;

namespace VoteWiselyBackend.Services
{
    public class AuthServices
    {
        private readonly Supabase.Client _supabaseClient;
        public AuthServices(Supabase.Client supabaseClient)
        {
            _supabaseClient = supabaseClient;
        }
        public (bool valid, string? message) ValidateSignUpRequest(SignUpRequest request)
        {
            if (string.IsNullOrEmpty(request.FullName) || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return (false, "All fields are required");
            }
            else if (request.Password != request.ConfirmPassword)
            {
                return (false, "Passwords do not match");
            }
            else if (request.Password.Length < 8)
            {
                return (false, "Password must be at least 8 characters long");
            }
            else if (!Regex.IsMatch(request.Email, @"^[^\s@]+@[^\s@]+\.[^\s@]+$"))
            {
                return (false, "Invalid email address");
            }
            return (true, null);
        }

        public async Task<Session?> SignUpUser(SignUpRequest request)
        {
            var authResponse = await _supabaseClient.Auth.SignUp(
                request.Email,
                request.Password,
                options: new SignUpOptions
                {
                    Data = new Dictionary<string, object>
                    {
                        { "full_name", request.FullName }
                    }
                }
            );

            return authResponse;
        }

        public bool ValidateAuthCredentials(LoginRequest request)
        {
            return !string.IsNullOrEmpty(request.Email) &&
                   !string.IsNullOrEmpty(request.Password);
        }

        public async Task<Session?> SignInUser(LoginRequest request)
        {
            var authResponse = await _supabaseClient.Auth.SignIn(
                request.Email,
                request.Password
            );
            return authResponse;
        }

        public async Task SignOut()
        {
            try
            {
                await _supabaseClient.Auth.SignOut();
            }
            catch (Exception)
            {
                throw new Exception("An error occurred during sign out");
            }
        }
    
        public async Task<ProviderAuthState?> SignInWithOAuth(OAuthSignInRequest request, Constants.Provider provider)
        {
            var authResponse = await _supabaseClient.Auth.SignIn(
                provider: provider,
                options: new SignInOptions
                {
                    RedirectTo = ""
                }
            );
            return authResponse;
        }
    }
}

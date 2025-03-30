using Microsoft.AspNetCore.Mvc;
using Supabase.Gotrue;
using Supabase.Interfaces;
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
        public bool ValidateSignUpRequest(SignUpRequest request)
        {
            return !string.IsNullOrEmpty(request.FullName) &&
                   !string.IsNullOrEmpty(request.Email) &&
                   !string.IsNullOrEmpty(request.Password) &&
                   request.Password == request.ConfirmPassword;
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
    }
}

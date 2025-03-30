using Microsoft.AspNetCore.Mvc;
using VoteWiselyBackend.Models;
using VoteWiselyBackend.Contracts;
using Supabase.Gotrue;

namespace VoteWiselyBackend.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : Controller
    {
        private readonly Supabase.Client _supabaseClient;

        public AuthController(Supabase.Client supabaseClient)
        {
            _supabaseClient = supabaseClient;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] SignUpRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.FullName) ||
                    string.IsNullOrEmpty(request.Email) ||
                    string.IsNullOrEmpty(request.Password))
                {
                    return BadRequest("All fields are required");
                }

                if (request.Password != request.ConfirmPassword)
                {
                    return BadRequest("Passwords do not match");
                }
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

                if (authResponse?.User == null)
                {
                    return BadRequest("Failed to create user");
                }

                return Ok(new { message = "User registered successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred during registration");
            }
        }
    }
}

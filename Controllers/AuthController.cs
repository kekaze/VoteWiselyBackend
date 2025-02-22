using Microsoft.AspNetCore.Mvc;
using VoteWiselyBackend.Models;

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

        public class SignUpRequest
        {
            public required string FullName { get; set; }
            public required string Email { get; set; }
            public required string Password { get; set; }
            public required string ConfirmPassword { get; set; }
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] SignUpRequest request)
        {
            try
            {
                // Validate request
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

                // Sign up user using Supabase Auth
                var authResponse = await _supabaseClient.Auth.SignUp(
                    request.Email,
                    request.Password
                );

                var session = _supabaseClient.Auth.CurrentSession;

                if (authResponse?.User == null)
                {
                    return BadRequest("Failed to create user");
                }

                // Insert into custom Users table
                var userData = new User
                {
                    Id = Guid.Parse(authResponse.User.Id),
                    FullName = request.FullName,
                    Email = request.Email.ToLower(),
                };

                var response = await _supabaseClient
                    .From<User>()
                    .Insert(userData);

                return Ok(new { message = "User registered successfully" });
            }
            catch (Exception ex)
            {
                // Log the error
                return StatusCode(500, "An error occurred during registration");
            }
        }
    }
}

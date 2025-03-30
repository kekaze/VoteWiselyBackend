using Microsoft.AspNetCore.Mvc;
using VoteWiselyBackend.Models;
using VoteWiselyBackend.Contracts;
using Supabase.Gotrue;
using VoteWiselyBackend.Services;

namespace VoteWiselyBackend.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : Controller
    {
        private readonly AuthServices _authServices;

        public AuthController(AuthServices authServices)
        {
            _authServices = authServices;
        }

        [HttpPost("Signup")]
        public async Task<IActionResult> SignUp([FromBody] SignUpRequest request)
        {
            try
            {
                bool validRequest = _authServices.ValidateSignUpRequest(request);
                if (!validRequest)
                {
                    return BadRequest("Something's wrong with your request");
                }

                var savedUser = await _authServices.SignUpUser(request);
                if (savedUser?.User == null)
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

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                bool validRequest = _authServices.ValidateAuthCredentials(request);
                if (!validRequest)
                {
                    return BadRequest("Something's wrong with your request");
                }

                var session = await _authServices.SignInUser(request);
                if (session == null)
                {
                    return BadRequest("Invalid credentials");
                }

                return Ok(new { message = "User logged in successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred during login");
            }
        }
    }
}

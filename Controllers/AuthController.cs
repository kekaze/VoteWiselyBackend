using Microsoft.AspNetCore.Mvc;
using VoteWiselyBackend.Contracts;
using VoteWiselyBackend.Services;
using Supabase.Gotrue.Exceptions;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

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
                var validatedRequest = await _authServices.ValidateSignUpRequest(request);
                if (!validatedRequest.valid)
                {
                    return BadRequest(new { validatedRequest.message });
                }

                var savedUser = await _authServices.SignUpUser(request);
                if (savedUser?.User == null)
                {
                    return BadRequest(new { message = "Failed to create user" } );
                }

                return Ok();
            }
            catch (GotrueException ex)
            {
                GoTrueExMessage? exceptionMessage = JsonConvert.DeserializeObject<GoTrueExMessage>(ex.Message);
                return StatusCode(422, new { message = exceptionMessage?.Msg });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred during registration" } );
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                bool validRequest = await _authServices.ValidateAuthCredentials(request);
                if (!validRequest)
                {
                    return BadRequest(new { message = "Something's wrong with your request" });
                }

                var session = await _authServices.SignInUser(request);
                if (session == null)
                {
                    return BadRequest(new { message = "Invalid credentials" });
                }

                HttpContext.Response.Cookies.Append("AccessToken", value: session.AccessToken!, options: new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTimeOffset.UtcNow.AddHours(1)
                });

                HttpContext.Response.Cookies.Append("RefreshToken", value: session.RefreshToken!, options: new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTimeOffset.UtcNow.AddHours(1)
                });

                return Ok(new { message = "User logged in successfully" });
            }
            catch (GotrueException ex)
            {
                if(ex.Reason == FailureHint.Reason.UserBadLogin)
                {
                    return StatusCode(400, new { message = "Invalid login credentials" });
                }
                else
                {
                    return StatusCode(400, new { message = "Email not yet confirmed" });
                }
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred during login" } );
            }
        }

        [Authorize]
        [HttpGet("CheckAuth")]
        public IActionResult CheckAuth()
        {
            return Ok(new { message = "User is authenticated" });
        }

        [HttpGet("SignOut")]
        public new async Task<IActionResult> SignOut()
        {
            await _authServices.SignOut();
            HttpContext.Response.Cookies.Delete("AccessToken");
            HttpContext.Response.Cookies.Delete("RefreshToken");
            return Ok(new { message = "User logged out successfully" });
        }
    }
}

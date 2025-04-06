﻿using Microsoft.AspNetCore.Mvc;
using VoteWiselyBackend.Models;
using VoteWiselyBackend.Contracts;
using Supabase.Gotrue;
using VoteWiselyBackend.Services;
using System.Security.Authentication;
using Supabase.Gotrue.Exceptions;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using OneOf.Types;

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
                var validatedRequest = _authServices.ValidateSignUpRequest(request);
                if (!validatedRequest.valid)
                {
                    return BadRequest(new { validatedRequest.message });
                }

                var savedUser = await _authServices.SignUpUser(request);
                if (savedUser?.User == null)
                {
                    return BadRequest(new { message = "Failed to create user" } );
                }

                HttpContext.Response.Cookies.Append("token", value: savedUser.AccessToken, options: new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTimeOffset.UtcNow.AddHours(1)
                });

                return Ok(new { message = "User registered successfully" });
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
            catch (GotrueException ex)
            {
                if(ex.Reason == FailureHint.Reason.UserBadLogin)
                {
                    return StatusCode(400, "Invalid login credentials");
                }
                else
                {
                    return StatusCode(400, "Email not yet confirmed");
                }
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred during login");
            }
        }
    }
}

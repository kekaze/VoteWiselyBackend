using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Supabase.Postgrest.Responses;
using VoteWiselyBackend.Contracts;
using VoteWiselyBackend.Models;
using VoteWiselyBackend.Services;

namespace VoteWiselyBackend.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ResultsController : Controller
    {
        private readonly SupabaseService _supabaseServices;
        public ResultsController(SupabaseService supabaseService)
        {
            _supabaseServices = supabaseService;
        }

        [HttpGet("{referenceId}")]
        public async Task<ActionResult> GetResult(Guid referenceId)
        {
            string? result = await _supabaseServices.GetRecommendation(referenceId);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}

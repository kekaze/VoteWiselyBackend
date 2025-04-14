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
        private readonly SupabaseServices _supabaseServices;
        public ResultsController(SupabaseServices supabaseService)
        {
            _supabaseServices = supabaseService;
        }

        [HttpGet("{resultReferenceId}")]
        public async Task<ActionResult> GetResult(Guid resultReferenceId)
        {
            string? result = await _supabaseServices.GetResult(resultReferenceId);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}

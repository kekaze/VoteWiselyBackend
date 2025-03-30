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

        [HttpPost("GetResult")]
        public async Task<List<Candidate>> GetResult([FromBody] GuidRequest request)
        {
            string? result = await _supabaseServices.GetResult(request.Reference);
            List<Candidate>? candidates = JsonConvert.DeserializeObject<List<Candidate>>(result);
            if (result == null)
            {
                return new List<Candidate>();
            }
            return candidates;
        }
    }
}

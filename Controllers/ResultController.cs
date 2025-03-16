using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Pinecone;
using VoteWiselyBackend.Contracts;
using VoteWiselyBackend.Models;
using VoteWiselyBackend.Services;

namespace VoteWiselyBackend.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ResultController : Controller
    {
        private readonly SupabaseServices _supabaseServices;
        public ResultController(SupabaseServices supabaseService)
        {
            _supabaseServices = supabaseService;
        }
        [HttpGet("{refId}")]
        public async Task<QueryResponse> GetSingleResult(Guid refId)
        {
            var result = await _supabaseServices.GetResult(refId);
            var resultJson = JsonConvert.DeserializeObject<QueryResponse>(result);

            return resultJson;
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VoteWiselyBackend.Contracts;

namespace VoteWiselyBackend.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class EmbeddingSearchController : Controller
    {
        public EmbeddingSearchController()
        {
            // constructor
        }

        [HttpPost("similarity-search")]
        public async Task<IActionResult> PerformSimilaritySearch([FromBody] PoliticalStance candidateCriteria)
        {
            Console.Write(candidateCriteria);
            return Ok(candidateCriteria);
        }
    }
}

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
            // Process candidateCriteria into a text in preparation for vectorization
            string inFavor = string.Join(", ", candidateCriteria.InFavor);
            string against = string.Join(", ", candidateCriteria.Against);
            string platforms = string.Join(", ", candidateCriteria.Platforms);

            string inFavorString = candidateCriteria.InFavor.Count == 0 ? "" : $"In favor to: {string.Join(", ", candidateCriteria.InFavor)}\n";
            string againtsString = candidateCriteria.Against.Count == 0 ? "" : $"Againts: {string.Join(", ", candidateCriteria.Against)}\n";
            string platformsString = candidateCriteria.Platforms.Count == 0 ? "" : $"Political Platform: {string.Join(", ", candidateCriteria.Platforms)}";
            string criteria = $"{inFavorString}{againtsString}{platformsString}";

            // Vectorize the text

            // Perform similarity search
            // Receive top 12 with most similarity
            // Prepare received data
            // Send clean data to front end
            return Ok(criteria);
        }
    }
}

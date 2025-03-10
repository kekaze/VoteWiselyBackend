using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using VoteWiselyBackend.Contracts;
using VoteWiselyBackend.Services;
using Pinecone;


namespace VoteWiselyBackend.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class EmbeddingSearchController : Controller
    {

        private readonly HttpClient _httpClient;
        private readonly PineconeService _pineconeService;
        private readonly JsonSerializerOptions _options;
        public EmbeddingSearchController(HttpClient httpClient, PineconeService pineconeService, JsonSerializerOptions jsonOptions)
        {
            _httpClient = httpClient;
            _pineconeService = pineconeService;
            _options = jsonOptions;
        }

        [HttpPost("similarity-search")]
        public async Task<IActionResult> PerformSimilaritySearch([FromBody] PoliticalStance candidateCriteria)
        {
            var similarCandidates = new Pinecone.QueryResponse();
            string criteria = DataTransformationServices.PrepareString(candidateCriteria);
            uint maxSentorialWinners = 12;

            // Vectorize the text
            var response = await _httpClient.PostAsJsonAsync("http://127.0.0.1:8000/embed", new { criteria });
            var responseJson = await response.Content.ReadFromJsonAsync<EmbeddingResponse>();

            // Perform similarity search
            if (responseJson != null)
            {
                similarCandidates = await _pineconeService.QueryIndexAsync(responseJson.Embedding, maxSentorialWinners);
            }

            // Receive top 12 with most similarity
            var matchedCandidates = JsonSerializer.Deserialize<PineconeMatchDto>(similarCandidates.ToString(), _options);

            // Prepare received data
            // Send clean data to front end 
            return Ok(matchedCandidates);
        }
    }
}

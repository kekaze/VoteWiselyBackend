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
        private readonly PineconeServices _pineconeService;
        private readonly JsonSerializerOptions _options;
        private readonly SupabaseServices _supabaseServices;
        public EmbeddingSearchController(HttpClient httpClient, PineconeServices pineconeService, JsonSerializerOptions jsonOptions, SupabaseServices supabaseServices)
        {
            _httpClient = httpClient;
            _pineconeService = pineconeService;
            _options = jsonOptions;
            _supabaseServices = supabaseServices;
        }

        [HttpPost("similarity-search")]
        public async Task<IActionResult> PerformSimilaritySearch([FromBody] PoliticalStance candidateCriteria)
        {
            var similarCandidates = new QueryResponse();
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

            // save the result and the selected candidate criteria to Supabase
            await _supabaseServices.SaveResults(similarCandidates.ToString());

            // handle exception errors
            return Ok();
        }
    }
}

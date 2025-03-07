using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Any;
using Newtonsoft.Json;
using System.Net.Http;
using VoteWiselyBackend.Contracts;
using VoteWiselyBackend.Services;


namespace VoteWiselyBackend.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class EmbeddingSearchController : Controller
    {

        private readonly HttpClient _httpClient;
        private readonly PineconeService _pineconeService;
        public EmbeddingSearchController(HttpClient httpClient, PineconeService pineconeService)
        {
            _httpClient = httpClient;
            _pineconeService = pineconeService;
        }

        [HttpPost("similarity-search")]
        public async Task<IActionResult> PerformSimilaritySearch([FromBody] PoliticalStance candidateCriteria)
        {
            var similarCandidates = new Pinecone.QueryResponse();
            string criteria = DataTransformationServices.PrepareString(candidateCriteria);

            // Vectorize the text
            var response = await _httpClient.PostAsJsonAsync("http://127.0.0.1:8000/embed", new { criteria });
            var responseJson = await response.Content.ReadFromJsonAsync<EmbeddingResponse>();

            // Perform similarity search
            if (responseJson != null)
            {
                similarCandidates = await _pineconeService.QueryIndexAsync(responseJson.Embedding, 2);
            }
            
            // Receive top 12 with most similarity
            // Prepare received data
            // Send clean data to front end 
            return Ok(similarCandidates.ToString());
        }
    }
}

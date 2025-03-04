using Microsoft.AspNetCore.Mvc;
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
        public EmbeddingSearchController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpPost("similarity-search")]
        public async Task<IActionResult> PerformSimilaritySearch([FromBody] PoliticalStance candidateCriteria)
        {
            string criteria = DataTransformationServices.PrepareString(candidateCriteria);

            // Vectorize the text
            var response = await _httpClient.PostAsJsonAsync("http://127.0.0.1:8000/embed", new { criteria });
            var responseJson = await response.Content.ReadFromJsonAsync<EmbeddingResponse>();

            // Perform similarity search

            // Receive top 12 with most similarity
            // Prepare received data
            // Send clean data to front end 
            return Ok(responseJson.Embedding);
        }
    }
}

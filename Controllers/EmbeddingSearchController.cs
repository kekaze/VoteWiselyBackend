using Microsoft.AspNetCore.Mvc;
using VoteWiselyBackend.Contracts;
using VoteWiselyBackend.Services;
using Pinecone;
using Microsoft.AspNetCore.Authorization;

namespace VoteWiselyBackend.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class EmbeddingSearchController : Controller
    {

        private readonly HttpClient _httpClient;
        private readonly PineconeServices _pineconeService;
        private readonly SupabaseServices _supabaseServices;
        private readonly DataTransformationServices _dataTransformationServices;
        public EmbeddingSearchController(HttpClient httpClient, PineconeServices pineconeService, SupabaseServices supabaseServices, DataTransformationServices dataTransformationServices)
        {
            _httpClient = httpClient;
            _pineconeService = pineconeService;
            _supabaseServices = supabaseServices;
            _dataTransformationServices = dataTransformationServices;
        }

        [Authorize]
        [HttpPost("CandidateSearch")]
        public async Task<IActionResult> PerformSimilaritySearch([FromBody] PoliticalStance candidateCriteria)
        {
            try
            {
                var cookies = Request.Cookies;
                await _supabaseServices.SetSession(cookies);

                uint maxSentorialWinners = 12;

                string criteria = DataTransformationServices.PrepareString(candidateCriteria);
                EmbeddingResponse transformedCriteria = await _dataTransformationServices.EmbedCriteria(criteria);
                List<ScoredVector> recommendedCandidates = await _pineconeService.QueryIndexAsync(transformedCriteria.Embedding, maxSentorialWinners);

                var resultModel = DataTransformationServices.CreateResultModel(recommendedCandidates);
                var saveResponse = await _supabaseServices.SaveResults(resultModel);

                return Ok(new { reference = saveResponse.Model!.Reference, result = saveResponse.Content });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}

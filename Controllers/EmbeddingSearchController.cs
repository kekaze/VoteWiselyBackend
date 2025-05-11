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
        private readonly PineconeService _pineconeService;
        private readonly SupabaseService _supabaseService;
        private readonly DataTransformationService _dataTransformationServices;
        public EmbeddingSearchController(HttpClient httpClient, PineconeService pineconeService, SupabaseService supabaseService, DataTransformationService dataTransformationServices)
        {
            _httpClient = httpClient;
            _pineconeService = pineconeService;
            _supabaseService = supabaseService;
            _dataTransformationServices = dataTransformationServices;
        }

        [Authorize]
        [HttpPost("CandidateSearch")]
        public async Task<IActionResult> PerformSimilaritySearch([FromBody] CandidateCriteria candidateCriteria)
        {
            try
            {
                var cookies = Request.Cookies;
                await _supabaseService.SetSession(cookies);

                string criteriaParagraph = DataTransformationService.CreateParagraph(candidateCriteria);
                EmbeddingResponse transformedCriteria = await _dataTransformationServices.EmbedCriteria(criteriaParagraph);
                List<ScoredVector> recommendedCandidates = await _pineconeService.QueryIndexAsync(transformedCriteria.Embedding, candidateCriteria);

                var resultModel = DataTransformationService.CreateResultModel(recommendedCandidates, candidateCriteria);
                var saveResponse = await _supabaseService.SaveRecommendation(resultModel);

                return Ok(new { reference = saveResponse.Model!.Reference, result = saveResponse.Content });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}

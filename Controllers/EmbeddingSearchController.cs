using Microsoft.AspNetCore.Mvc;
using VoteWiselyBackend.Contracts;
using VoteWiselyBackend.Services;
using VoteWiselyBackend.Models;
using Pinecone;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Supabase.Gotrue;

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
        [HttpPost("similarity-search")]
        public async Task<IActionResult> PerformSimilaritySearch([FromBody] PoliticalStance candidateCriteria)
        {
            try
            {
                var cookies = Request.Cookies;
                await _supabaseServices.SetSession(cookies);

                var resultModel = new List<Result>();
                uint maxSentorialWinners = 12;

                string criteria = DataTransformationServices.PrepareString(candidateCriteria);
                EmbeddingResponse transformedCriteria = await _dataTransformationServices.EmbedCriteria(criteria);
                List<ScoredVector> recommendedCandidates = await _pineconeService.QueryIndexAsync(transformedCriteria.Embedding, maxSentorialWinners);

                Guid resultReference = InfrastructureService.GetGuid();
                foreach (var candidate in recommendedCandidates)
                {
                    if (candidate.Metadata != null)
                    {
                        resultModel.Add(
                            new Result
                            {
                                Reference = resultReference,
                                Score = (float)candidate.Score!,
                                CandidateName = $"#{candidate.Metadata["ballot_number"]!.Value} {candidate.Metadata["name"]!.Value}",
                                PoliticalParty = (string)candidate.Metadata["political_party"]!.Value,
                                Type = "admin_event"
                            }
                        );
                    }
                }
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

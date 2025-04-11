using Microsoft.AspNetCore.Mvc;
using VoteWiselyBackend.Contracts;
using VoteWiselyBackend.Services;
using VoteWiselyBackend.Models;
using Pinecone;

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

        [HttpPost("similarity-search")]
        public async Task<IActionResult> PerformSimilaritySearch([FromBody] PoliticalStance candidateCriteria)
        {
            string criteria = DataTransformationServices.PrepareString(candidateCriteria);
            var similarCandidates = new QueryResponse();
            var resultVectorSearch = new List<ScoredVector>();
            var resultModel = new List<Result>();
            uint maxSentorialWinners = 12;

            EmbeddingResponse responseJson = await _dataTransformationServices.EmbedCriteria(criteria);

            // Perform similarity search
            if (responseJson != null)
            {
                similarCandidates = await _pineconeService.QueryIndexAsync(responseJson.Embedding, maxSentorialWinners);
            }

            // save the result and the selected candidate criteria to Supabase
            if (similarCandidates.Matches != null)
            {
                resultVectorSearch = similarCandidates.Matches.ToList();
            }

            Guid resultReference = Guid.NewGuid();
            foreach (var result in resultVectorSearch)
            {
                if (result.Metadata != null)
                {
                    resultModel.Add(
                        new Result
                        {
                            Reference = resultReference,
                            Score = (float)result.Score,
                            CandidateName = $"#{result.Metadata["ballot_number"].Value} {result.Metadata["name"].Value}",
                            PoliticalParty = (string)result.Metadata["political_party"].Value,
                            Type = "admin_event"
                        }
                    );
                }
            }
            var saveResponse = await _supabaseServices.SaveResults(resultModel);

            // handle exception errors
            return Ok();
        }
    }
}

using Pinecone;
using VoteWiselyBackend.Contracts;
using VoteWiselyBackend.Extensions;
using VoteWiselyBackend.Models;

namespace VoteWiselyBackend.Services
{
    public class DataTransformationServices
    {
        private readonly HttpClient _httpClient;
        public DataTransformationServices(HttpClient httpClient) 
        {
            _httpClient = httpClient;
        }
        public static string PrepareString(PoliticalStance PoliticalCriteriaTitleAndValuePair)
        {
            var labelMap = new Dictionary<string, string>
            {
                { "InFavor", "In favor to" },
                { "Against", "Against" },
                { "Platforms", "Political Platform" }
                // To add other properties in the future
            };

            var sections = new List<string>();

            foreach (var prop in typeof(PoliticalStance).GetProperties())
            {
                // It is necessary to make sure that we are working with a list of strings for the below logic to work
                if (prop.PropertyType.IsListOfType(typeof(string)))
                {
                    var collection = prop.GetValue(PoliticalCriteriaTitleAndValuePair) as IEnumerable<string>;

                    if (collection?.Any() == true)
                    {
                        string label = labelMap.TryGetValue(prop.Name, out var displayLabel)
                            ? displayLabel
                            : prop.Name;

                        sections.Add($"{label}: {string.Join(", ", collection)}");
                    }
                }
            }
            return string.Join("\n", sections);
        }

        public async Task<EmbeddingResponse> EmbedCriteria(string criteria)
        {
            try
            {
                if (string.IsNullOrEmpty(criteria))
                {
                    throw new ArgumentException("Criteria cannot be null or empty.", nameof(criteria));
                }

                var embeddingString = await _httpClient.PostAsJsonAsync("http://127.0.0.1:8000/embed", new { criteria });
                var embeddingObject = await embeddingString.Content.ReadFromJsonAsync<EmbeddingResponse>();

                return embeddingObject!;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while embedding criteria.", ex);
            }
        }

        public static List<Result> CreateResultModel(List<ScoredVector> recommendedCandidates)
        {
            Guid resultReference = InfrastructureService.GetGuid();
            var resultModel = new List<Result>();
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
            return resultModel;
        }
    }
}

using Pinecone;
using VoteWiselyBackend.Contracts;

namespace VoteWiselyBackend.Services
{
    public class PineconeServices
    {
        private readonly PineconeClient _pineconeClient;
        private readonly string _indexHost;

        public PineconeServices(PineconeClient pineconeClient, string indexHost)
        {
            _pineconeClient = pineconeClient;
            _indexHost = indexHost;
        }

        public async Task<List<ScoredVector>> QueryIndexAsync(float[] queryVector, CandidateCriteria candidateCriteria)
        {
            try
            {
                uint maxSentorialWinners = 12;
                var index = _pineconeClient.Index(host: _indexHost);
                var queryRequest = new QueryRequest
                {
                    Vector = queryVector,
                    TopK = maxSentorialWinners,
                    IncludeMetadata = true,
                    Namespace = "senatorial_candidates_2025",
                    Filter = new Metadata
                    {
                        ["not_political_dynasty"] = candidateCriteria.NotPoliticalDynasty,
                        ["no_criminal_records"] = candidateCriteria.NoCriminalRecords
                    }
                };

                var queryResponse = await index.QueryAsync(queryRequest);
                return queryResponse.Matches!.ToList();
            }
            catch
            {
                throw new Exception("Failed to query the index");
            }
        }

    }
}

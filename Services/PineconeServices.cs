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

        public async Task<List<ScoredVector>> QueryIndexAsync(float[] queryVector, uint topK = 5)
        {
            try
            {
                var index = _pineconeClient.Index(host: _indexHost);
                var queryRequest = new QueryRequest
                {
                    Vector = queryVector,
                    TopK = topK,
                    IncludeMetadata = true,
                    Namespace = "senatorial_candidates_2025"
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

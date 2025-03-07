using Pinecone;
using VoteWiselyBackend.Contracts;

namespace VoteWiselyBackend.Services
{
    public class PineconeService
    {
        private readonly PineconeClient _pineconeClient;
        private readonly string _indexHost;

        public PineconeService(PineconeClient pineconeClient, string indexHost)
        {
            _pineconeClient = pineconeClient;
            _indexHost = indexHost;
        }

        public async Task<QueryResponse> QueryIndexAsync(float[] queryVector, uint topK = 5)
        {
            var index = _pineconeClient.Index(host: _indexHost);

            var queryRequest = new QueryRequest
            {
                TopK = topK,
                Vector = queryVector,
                IncludeMetadata = true
            };
            var response = await index.QueryAsync(
                new QueryRequest
                {
                    Vector = queryVector,
                    TopK = topK,
                    IncludeMetadata = true,
                    Namespace = "senators_2025"
                }
            );
            return response;
        }

    }
}

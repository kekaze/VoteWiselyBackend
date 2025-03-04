using Pinecone;

namespace VoteWiselyBackend.Services
{
    public class PineconeService
    {
        private readonly PineconeClient _pineconeClient;
        private readonly string _indexName;

        public PineconeService(PineconeClient pineconeClient, string indexName)
        {
            _pineconeClient = pineconeClient;
            _indexName = indexName;
        }

        public async Task QueryIndexAsync(float[] queryVector, uint topK = 5)
        {
            var index = _pineconeClient.Index(_indexName);

            var queryRequest = new QueryRequest
            {
                TopK = topK,
                Vector = queryVector,
                IncludeMetadata = true
            };

            var response = await index.QueryAsync(
                new QueryRequest
                {
                    TopK = topK,
                    Vector = queryVector,
                    IncludeMetadata = true
                }
            );
        }

    }
}

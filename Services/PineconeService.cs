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
    }
}

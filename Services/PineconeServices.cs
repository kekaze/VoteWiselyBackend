﻿using Pinecone;
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

        public async Task<QueryResponse> QueryIndexAsync(float[] queryVector, uint topK = 5)
        {
            var index = _pineconeClient.Index(host: _indexHost);
            var queryRequest = new QueryRequest
            {
                Vector = queryVector,
                TopK = topK,
                IncludeMetadata = true,
                Namespace = "senators_2025"
            };
            return await index.QueryAsync(queryRequest);
        }

    }
}

using Supabase.Postgrest.Responses;
using System.Text.Json;
using VoteWiselyBackend.Contracts;
using VoteWiselyBackend.Models;
using System.Diagnostics;

namespace VoteWiselyBackend.Services
{
    public class SupabaseServices
    {
        private readonly Supabase.Client _supabaseClient;
        public SupabaseServices(Supabase.Client supabaseClient)
        {
            _supabaseClient = supabaseClient;
        }
        public async Task<ModeledResponse<Result>> SaveResults(List<Result> resultModel)
        {
            try
            {
                return await _supabaseClient.From<Result>().Insert(resultModel);
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
                throw;
            }
        }

        public async Task<string?> GetResult(Guid referenceId)
        {
            var queryResponse = await _supabaseClient
                .From<Result>()
                .Select(x => new object[] { x.CandidateName, x.PoliticalParty, x.Score })
                .Where(x => x.Reference == referenceId)
                .Get();
            return queryResponse.Content;
        }
    }
}

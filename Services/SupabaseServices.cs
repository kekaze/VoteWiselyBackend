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
        public async Task SaveResults(string matchedCandidatesString)
        {
            try
            {
                var model = new Result
                {
                    ResultString = matchedCandidatesString,
                    Type = "admin_event"
                };

                await _supabaseClient.From<Result>().Insert(model);
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
                throw;
            }
        }

        public async Task<string?> GetResult(Guid refId)
        {
            var result = await _supabaseClient
                .From<Result>()
                .Select(x => new object[] { x.ResultString })
                .Where(x => x.Reference == refId)
                .Get();
            
            return result.Model.ResultString;
        }
    }
}

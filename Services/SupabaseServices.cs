using System.Text.Json;
using VoteWiselyBackend.Contracts;
using VoteWiselyBackend.Models;

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
    }
}

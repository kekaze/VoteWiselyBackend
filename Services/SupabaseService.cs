using Supabase.Postgrest.Responses;
using VoteWiselyBackend.Models;

namespace VoteWiselyBackend.Services
{
    public class SupabaseService
    {
        private readonly Supabase.Client _supabaseClient;
        public SupabaseService(Supabase.Client supabaseClient)
        {
            _supabaseClient = supabaseClient;
        }
        public async Task<ModeledResponse<Result>> SaveRecommendation(Result resultModel)
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

        public async Task<string?> GetRecommendation(Guid referenceId)
        {
            var queryResponse = await _supabaseClient
                .From<Result>()
                .Select(x => new object[] { x.Id, x.Criteria, x.Recommendation })
                .Where(x => x.Reference == referenceId)
                .Limit(1)
                .Get();
            return queryResponse.Content;
        }

        public async Task SetSession(IRequestCookieCollection cookies)
        {
            await _supabaseClient.Auth.SetSession(cookies["AccessToken"]!, cookies["RefreshToken"]!);
        }
    }
}

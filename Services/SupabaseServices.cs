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

        //public async Task<string?> GetResult(Guid refId)
        //{
        //    var result = await _supabaseClient
        //        .From<Result>()
        //        .Select(x => new object[] { x.ResultString })
        //        .Where(x => x.Reference == refId)
        //        .Get();
            
        //    return result.Model.ResultString;
        //}
    }
}

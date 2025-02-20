namespace VoteWiselyBackend.Factories.Interfaces
{
    public interface ISupabaseClientFactory
    {
        Task<Supabase.Client> CreateClientAsync();
    }
}

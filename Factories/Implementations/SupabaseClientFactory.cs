using VoteWiselyBackend.Factories.Interfaces;

namespace VoteWiselyBackend.Factories.Implementations
{
    public class SupabaseClientFactory : ISupabaseClientFactory
    {
        private readonly IConfiguration _configuration;

        public SupabaseClientFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<Supabase.Client> CreateClientAsync()
        {
            var supabaseUrl = _configuration["Supabase:Url"]
                ?? Environment.GetEnvironmentVariable("SUPABASE_URL");
            var supabaseKey = _configuration["Supabase:ApiKey"]
                ?? Environment.GetEnvironmentVariable("SUPABASE_KEY");

            if (string.IsNullOrEmpty(supabaseUrl) || string.IsNullOrEmpty(supabaseKey))
            {
                throw new InvalidOperationException("Supabase configuration is missing");
            }

            var supabase = new Supabase.Client(
                supabaseUrl,
                supabaseKey,
                new Supabase.SupabaseOptions { AutoConnectRealtime = true }
            );

            try
            {
                await supabase.InitializeAsync();
                return supabase;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to initialize Supabase client", ex);
            }
        }
    }
}

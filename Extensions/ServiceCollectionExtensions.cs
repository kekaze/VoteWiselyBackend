using VoteWiselyBackend.Factories.Interfaces;

namespace VoteWiselyBackend.Extensions
{
    public static class SupabaseExtensions
    {
        public static async Task<IServiceCollection> AddSupabaseClientAsync(
            this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var factory = serviceProvider.GetRequiredService<ISupabaseClientFactory>();
            var client = await factory.CreateClientAsync();
            services.AddSingleton(client);
            return services;
        }
    }
}

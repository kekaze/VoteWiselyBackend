namespace VoteWiselyBackend.Services
{
    public class InfrastructureService
    {
        public static Guid GetGuid()
        {
            return Guid.NewGuid();
        }
    }
}

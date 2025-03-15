using System.Text.Json.Serialization;

namespace VoteWiselyBackend.Contracts
{
    public class PineconeMatchDto
    {
        public List<MatchDto> Matches { get; set; } = new();
    }

    public class MatchDto
    {
        public string Id { get; set; }
        public float Score { get; set; }
        public MetadataDto Metadata { get; set; }
    }

    public class MetadataDto
    {
        public string Against { get; set; }
        [JsonPropertyName("ballot_number")]
        public string BallotNumber { get; set; }
        [JsonPropertyName("has_reservations")]
        public string HasReservations { get; set; }
        [JsonPropertyName("in_favor")]
        public string InFavor { get; set; }
        public string Name { get; set; }
        public string Platforms { get; set; }
        [JsonPropertyName("political_party")]
        public string PoliticalParty { get; set; }
    }
}

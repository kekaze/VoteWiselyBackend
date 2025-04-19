using System.Text.Json.Serialization;

namespace VoteWiselyBackend.Contracts
{
    public class CandidateCriteria
    {
        [JsonPropertyName("in_favor")]
        public List<string> InFavor { get; set; } = new List<string>();

        public List<string> Against { get; set; } = new List<string>();

        public List<string> Platforms { get; set; } = new List<string>();
        [JsonPropertyName("with_reservations")]
        public List<string> WithReservations { get; set; } = new List<string>();
        [JsonPropertyName("not_political_dynasty")]
        public bool? NotPoliticalDynasty { get; set; } = null;
        [JsonPropertyName("no_criminal_records")]
        public bool? NoCriminalRecords { get; set; } = null;
    }
}

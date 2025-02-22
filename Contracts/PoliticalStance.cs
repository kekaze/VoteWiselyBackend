using System.Text.Json.Serialization;

namespace VoteWiselyBackend.Contracts
{
    public class PoliticalStance
    {
        [JsonPropertyName("in_favor")]
        public List<string> InFavor { get; set; } = new List<string>();

        public List<string> Against { get; set; } = new List<string>();

        public List<string> Platforms { get; set; } = new List<string>();
    }
}

using Newtonsoft.Json;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using VoteWiselyBackend.Contracts;

namespace VoteWiselyBackend.Models
{
    [Table("recommendations")]
    public class Result : BaseModel
    {
        [PrimaryKey("id")]
        [JsonIgnore]
        public int Id { get; set; }
        [Column("reference")]
        public Guid Reference { get; set; }
        [Column("criteria")]
        public PoliticalStance Criteria { get; set; }
        [Column("recommendation")]
        public List<Recommendation> Recommendation { get; set; }
        [Column("type")]
        public string? Type { get; set; }
    }

    public class Recommendation
    {
        public string CandidateName { get; set; } = string.Empty;
        public float Score { get; set; }
        public string PoliticalParty { get; set; } = string.Empty;
    }
}

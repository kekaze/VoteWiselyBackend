using Newtonsoft.Json;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace VoteWiselyBackend.Models
{
    [Table("phelection_results")]
    public class Result : BaseModel
    {
        [PrimaryKey("id")]
        [JsonIgnore]
        public int Id { get; set; }
        [Column("reference")]
        public Guid Reference { get; set; }
        [Column("score")]
        public float Score { get; set; }
        [Column("candidate_name")]
        public string? CandidateName { get; set; }
        [Column("political_party")]
        public string? PoliticalParty { get; set; }
        [Column("type")]
        public string? Type { get; set; }
    }
}

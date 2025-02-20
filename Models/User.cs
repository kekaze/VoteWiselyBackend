using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace VoteWiselyBackend.Models
{
    [Table("users")]
    public class User : BaseModel
    {
        [PrimaryKey("id")]
        public required string Id { get; set; }
        [Column("name")]
        public required string FullName { get; set; }
        [Column("email")]
        public required string Email { get; set; }
        [Column("create_at")]
        public DateTime CreatedAt { get; set; }
    }
}

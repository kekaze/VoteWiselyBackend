using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace VoteWiselyBackend.Models
{
    [Table("users")]
    public class User : BaseModel
    {
        [PrimaryKey("id")]
        public Guid Id { get; set; }
        [Column("full_name")]
        public string FullName { get; set; }
        [Column("email")]
        public string Email { get; set; }
    }
}

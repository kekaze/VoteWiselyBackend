using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace VoteWiselyBackend.Models
{
    [Table("results")]
    public class Result : BaseModel
    {
        [PrimaryKey("id")]
        public int Id { get; set; }
        [Column("reference")]
        public Guid? Reference { get; set; }
        [Column("result")]
        public string ResultString { get; set; }
        [Column("type")]
        public string Type { get; set; }
        //[Column("created_by")]
        //public Guid created_by { get; set; }
    }
}

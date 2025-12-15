using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [Table("useraction")]
    public class UserAction
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("action_type")]
        public int ActionType { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        public ActionResourceMetric ActionResourceMetric { get; set; }
    }
}

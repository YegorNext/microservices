using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WebApplication1.Models
{
    [Table("actionresourcemetric")]
    public class ActionResourceMetric
    {
        [Key] // PK = id
        [Column("id")]
        public int Id { get; set; }

        [Column("action_id")] // FK → useraction.id
        public int ActionId { get; set; }

        [Column("cpu_time")]
        public int CpuTime { get; set; }

        [Column("ram_usage_mb")]
        public decimal RamUsageMb { get; set; }

        [Column("response_time_ms")]
        public int ResponseTimeMs { get; set; }

        [Column("collected_at")]
        public DateTime CollectedAt { get; set; }

        [ForeignKey(nameof(ActionId))]
        [JsonIgnore]
        public UserAction? UserAction { get; set; }

    }
}

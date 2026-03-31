using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstAidPlus.Models
{
    [Table("user_game_progress")]
    public class UserGameProgress
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("situation_id")]
        public int SituationId { get; set; }

        [Column("is_completed")]
        public bool IsCompleted { get; set; }

        [Column("score_earned")]
        public int ScoreEarned { get; set; }

        [Column("completed_at")]
        public DateTime CompletedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public User? User { get; set; }

        [ForeignKey("SituationId")]
        public GameSituation? Situation { get; set; }
    }
}

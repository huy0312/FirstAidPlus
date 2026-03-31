using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstAidPlus.Models
{
    [Table("game_options")]
    public class GameOption
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("situation_id")]
        public int SituationId { get; set; }

        [Required]
        [Column("option_text")]
        public string OptionText { get; set; } = string.Empty;

        [Column("is_correct")]
        public bool IsCorrect { get; set; }

        [Column("explanation")]
        public string? Explanation { get; set; }

        [Column("points")]
        public int Points { get; set; } = 0;

        [Column("icon_url")]
        public string? IconUrl { get; set; }

        [ForeignKey("SituationId")]
        public GameSituation? Situation { get; set; }
    }
}

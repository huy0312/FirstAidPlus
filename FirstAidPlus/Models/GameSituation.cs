using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstAidPlus.Models
{
    [Table("game_situations")]
    public class GameSituation
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("category_id")]
        public int CategoryId { get; set; }

        [Required]
        [Column("title")]
        public string Title { get; set; } = string.Empty;

        [Column("character_context")]
        public string? CharacterContext { get; set; }

        [Column("situation_description")]
        public string? SituationDescription { get; set; }

        [Required]
        [Column("question")]
        public string Question { get; set; } = string.Empty;

        [Column("image_url")]
        public string? ImageUrl { get; set; }

        [ForeignKey("CategoryId")]
        public FamilyCourseCategory? Category { get; set; }

        public ICollection<GameOption> Options { get; set; } = new List<GameOption>();
    }
}

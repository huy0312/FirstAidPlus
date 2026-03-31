using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstAidPlus.Models
{
    [Table("game_category_experts")]
    public class GameCategoryExpert
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("expert_id")]
        public int ExpertId { get; set; }

        [Column("category_id")]
        public int CategoryId { get; set; }

        [Column("assigned_at")]
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("ExpertId")]
        public virtual User? Expert { get; set; }

        [ForeignKey("CategoryId")]
        public virtual FamilyCourseCategory? Category { get; set; }
    }
}

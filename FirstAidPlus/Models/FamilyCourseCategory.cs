using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstAidPlus.Models
{
    [Table("family_course_categories")]
    public class FamilyCourseCategory
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("title")]
        public string Title { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Column("icon_url")]
        public string? IconUrl { get; set; }

        [Column("color_hex")]
        public string? ColorHex { get; set; }

        public ICollection<GameSituation> Situations { get; set; } = new List<GameSituation>();
    }
}

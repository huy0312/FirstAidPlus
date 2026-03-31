using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstAidPlus.Models
{
    public class CommentReaction
    {
        [Key]
        public int Id { get; set; }

        public int CommentId { get; set; }
        [ForeignKey("CommentId")]
        public LessonComment? Comment { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }

        public string ReactionType { get; set; } = "👍"; // 👍 ❤️ 😂 😮 😢
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

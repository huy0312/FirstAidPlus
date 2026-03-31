using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstAidPlus.Models
{
    public class LessonComment
    {
        [Key]
        public int Id { get; set; }

        public int LessonId { get; set; }
        [ForeignKey("LessonId")]
        public CourseLesson? Lesson { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }

        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Parent comment for threading (optional for now, but good for future)
        public int? ParentId { get; set; }
        [ForeignKey("ParentId")]
        public LessonComment? Parent { get; set; }
        
        public ICollection<LessonComment>? Replies { get; set; }
        public ICollection<CommentReaction> Reactions { get; set; } = new List<CommentReaction>();
    }
}

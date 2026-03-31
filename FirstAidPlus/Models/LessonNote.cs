using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstAidPlus.Models
{
    public class LessonNote
    {
        [Key]
        public int Id { get; set; }

        public int LessonId { get; set; }
        [ForeignKey("LessonId")]
        public CourseLesson? Lesson { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        // Optional: track where in the video the note was made
        public double? VideoTimestamp { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}

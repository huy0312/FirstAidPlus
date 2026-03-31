using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstAidPlus.Models
{
    public class UserLessonProgress
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        public int LessonId { get; set; }
        [ForeignKey("LessonId")]
        public CourseLesson Lesson { get; set; }

        public int TimeSpentSeconds { get; set; } = 0;

        public bool IsCompleted { get; set; } = false;

        public DateTime LastAccessed { get; set; } = DateTime.UtcNow;
    }
}

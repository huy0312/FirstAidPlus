using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstAidPlus.Models
{
    public class CourseSyllabus
    {
        [Key]
        public int Id { get; set; }
        public int CourseId { get; set; }
        [ForeignKey("CourseId")]
        public Course Course { get; set; }
        public string Title { get; set; }
        public string? Duration { get; set; }
        public int LessonCount { get; set; }

        public List<CourseLesson> Lessons { get; set; } = new List<CourseLesson>();
    }
}

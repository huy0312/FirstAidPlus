using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstAidPlus.Models
{
    public class CourseLesson
    {
        [Key]
        public int Id { get; set; }

        public int SyllabusId { get; set; }
        
        [ForeignKey("SyllabusId")]
        public CourseSyllabus Syllabus { get; set; }

        public string Title { get; set; }
        
        public string Type { get; set; } = "Reading"; // Video, Reading

        public string? Content { get; set; } // For Reading
        public string? Description { get; set; } // Short summary/intro for all types

        public string? VideoUrl { get; set; } // For Video

        public int Duration { get; set; } // Min

        public int OrderIndex { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

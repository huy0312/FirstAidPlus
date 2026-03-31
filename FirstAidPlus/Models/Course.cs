using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace FirstAidPlus.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? CertificateName { get; set; }
        public string? ImageUrl { get; set; }
        public string? VideoUrl { get; set; }
        public string? TrainingImageUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsPopular { get; set; }

        public int? InstructorId { get; set; }
        [ForeignKey("InstructorId")]
        public virtual User? Instructor { get; set; }
        
        public ICollection<CourseObjective> Objectives { get; set; } = new List<CourseObjective>();
        public ICollection<CourseSyllabus> Syllabus { get; set; } = new List<CourseSyllabus>();
        public ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
        public ICollection<PlanCourse> PlanCourses { get; set; } = new List<PlanCourse>();

        // Dynamic Duration Calculation
        public int TotalDurationMinutes => (Syllabus != null) ? Syllabus.SelectMany(s => s.Lessons ?? new List<CourseLesson>()).Sum(l => l.Duration) : 0;

        public string GetFormattedDuration()
        {
            int totalMinutes = TotalDurationMinutes;
            if (totalMinutes == 0) return "Chưa có bài học";
            
            int hours = totalMinutes / 60;
            int minutes = totalMinutes % 60;

            if (hours > 0)
            {
                return minutes > 0 ? $"{hours} giờ {minutes} phút" : $"{hours} giờ";
            }
            return $"{minutes} phút";
        }
    }
}

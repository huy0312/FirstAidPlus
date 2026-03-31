using FirstAidPlus.Models;

namespace FirstAidPlus.ViewModels
{
    public class CourseDetailViewModel
    {
        public Course Course { get; set; }
        public bool IsEnrolled { get; set; }
        public bool HasDirectEnrollment { get; set; }
        public List<string> Objectives { get; set; } = new List<string>();
        public List<SyllabusItem> Syllabus { get; set; } = new List<SyllabusItem>();
        public List<Course> RelatedCourses { get; set; } = new List<Course>();
        public List<Feedback> Feedbacks { get; set; } = new List<Feedback>();
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public int? FirstLessonId { get; set; }
    }

    public class SyllabusItem
    {
        public string Title { get; set; }
        public string Duration { get; set; }
        public int LessonCount { get; set; }
    }
}

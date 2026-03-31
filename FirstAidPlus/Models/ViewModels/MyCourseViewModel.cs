using System.ComponentModel.DataAnnotations;

namespace FirstAidPlus.Models.ViewModels
{
    public class MyCourseViewModel
    {
        public int CourseId { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public DateTime EnrolledAt { get; set; }
        public string Status { get; set; }
        public int? InstructorId { get; set; }
        
        public int TotalLessons { get; set; }
        public int CompletedLessons { get; set; }
        
        public int ProgressPercentage 
        { 
            get 
            {
                if (TotalLessons == 0) return 0;
                return (int)((double)CompletedLessons / TotalLessons * 100);
            }
        }
    }
}

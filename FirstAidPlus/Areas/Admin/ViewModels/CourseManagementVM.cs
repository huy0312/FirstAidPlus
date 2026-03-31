using FirstAidPlus.Models;

namespace FirstAidPlus.Areas.Admin.ViewModels
{
    public class CourseManagementVM
    {
        public IEnumerable<Course> Courses { get; set; } = new List<Course>();
        
        // Chart data
        public List<string> PopularityLabels { get; set; } = new();
        public List<int> PopularityData { get; set; } = new();
        
        public List<string> RatingLabels { get; set; } = new();
        public List<double> RatingData { get; set; } = new();
    }
}

using FirstAidPlus.Models;

namespace FirstAidPlus.Areas.Admin.ViewModels
{
    public class AdminDashboardVM
    {
        public int TotalUsers { get; set; }
        public int TotalCourses { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalReviews { get; set; }
        public double AverageRating { get; set; }


        public List<string> MonthLabels { get; set; } = new();
        public List<int> RegistrationData { get; set; } = new();
        public List<decimal> RevenueData { get; set; } = new();

        public List<string> RoleLabels { get; set; } = new();
        public List<int> RoleData { get; set; } = new();

        public List<Transaction> RecentTransactions { get; set; } = new();
        public List<Feedback> RecentFeedbacks { get; set; } = new();

        public List<CourseRatingDTO> CourseRatings { get; set; } = new();

        public List<string> CoursePopularityLabels { get; set; } = new();
        public List<int> CoursePopularityData { get; set; } = new();
    }


    public class CourseRatingDTO
    {
        public string CourseTitle { get; set; } = "";
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
    }
}



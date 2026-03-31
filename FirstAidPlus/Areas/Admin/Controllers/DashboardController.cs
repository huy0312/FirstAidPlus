using FirstAidPlus.Areas.Admin.ViewModels;
using FirstAidPlus.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FirstAidPlus.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new AdminDashboardVM();

            // Summary Stats
            vm.TotalUsers = await _context.Users.CountAsync();
            vm.TotalCourses = await _context.Courses.CountAsync();
            
            var courseRevenue = await _context.Enrollments
                .Where(e => e.Status == "Active" || e.Status == "Completed")
                .SumAsync(e => e.Amount);
                
            var subscriptionRevenue = await _context.Transactions
                .Where(t => t.Status == "Success")
                .SumAsync(t => t.Amount);
                
            vm.TotalRevenue = courseRevenue + subscriptionRevenue;

            // Review Stats
            vm.TotalReviews = await _context.Feedbacks.CountAsync();
            vm.AverageRating = vm.TotalReviews > 0 
                ? await _context.Feedbacks.AverageAsync(f => f.Rating) 
                : 0;

            // Chart: Last 6 Months
            for (int i = 5; i >= 0; i--)
            {
                var date = DateTime.UtcNow.AddMonths(-i);
                vm.MonthLabels.Add(date.ToString("MM/yyyy"));

                var startOfMonth = new DateTime(date.Year, date.Month, 1);
                var endOfMonth = startOfMonth.AddMonths(1);

                vm.RegistrationData.Add(await _context.Users.CountAsync(u => u.CreatedAt >= startOfMonth && u.CreatedAt < endOfMonth));
                
                var monthlyCourseRev = await _context.Enrollments
                    .Where(e => e.EnrolledAt >= startOfMonth && e.EnrolledAt < endOfMonth && (e.Status == "Active" || e.Status == "Completed"))
                    .SumAsync(e => e.Amount);
                    
                var monthlySubRev = await _context.Transactions
                    .Where(t => t.CreatedAt >= startOfMonth && t.CreatedAt < endOfMonth && t.Status == "Success")
                    .SumAsync(t => t.Amount);
                    
                vm.RevenueData.Add(monthlyCourseRev + monthlySubRev);
            }

            // Role Distribution
            var roles = await _context.Users
                .Include(u => u.Role)
                .GroupBy(u => u.Role != null ? u.Role.RoleName : "N/A")
                .Select(g => new { Role = g.Key, Count = g.Count() })
                .ToListAsync();
            vm.RoleLabels = roles.Select(r => r.Role).ToList();
            vm.RoleData = roles.Select(r => r.Count).ToList();

            // Recent Transactions
            vm.RecentTransactions = await _context.Transactions
                .Include(t => t.User)
                .Include(t => t.Plan)
                .OrderByDescending(t => t.CreatedAt)
                .Take(5)
                .ToListAsync();

            // Recent Feedbacks
            vm.RecentFeedbacks = await _context.Feedbacks
                .Include(f => f.User)
                .Include(f => f.Course)
                .OrderByDescending(f => f.CreatedAt)
                .Take(5)
                .ToListAsync();

            // Course Ratings Overview
            vm.CourseRatings = await _context.Courses
                .Include(c => c.Feedbacks)
                .Select(c => new CourseRatingDTO
                {
                    CourseTitle = c.Title,
                    AverageRating = c.Feedbacks.Any() ? c.Feedbacks.Average(f => f.Rating) : 0,
                    ReviewCount = c.Feedbacks.Count()
                })
                .OrderByDescending(c => c.AverageRating)
                .ToListAsync();

            // Course Popularity Overview (Enrollments)
            var popularity = await _context.Enrollments
                .Include(e => e.Course)
                .Where(e => e.Course != null)
                .GroupBy(e => e.Course.Title)
                .Select(g => new { Title = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToListAsync();
            
            vm.CoursePopularityLabels = popularity.Select(p => p.Title).ToList();
            vm.CoursePopularityData = popularity.Select(p => p.Count).ToList();

            return View(vm);
        }



    }
}

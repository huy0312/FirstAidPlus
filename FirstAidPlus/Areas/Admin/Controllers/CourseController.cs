using FirstAidPlus.Areas.Admin.ViewModels;
using FirstAidPlus.Data;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FirstAidPlus.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CourseController : Controller
    {
        private readonly AppDbContext _context;

        public CourseController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var courses = await _context.Courses
                .Include(c => c.Feedbacks)
                .ToListAsync();

            var vm = new CourseManagementVM
            {
                Courses = courses
            };

            // Popularity data (Top 10)
            var popularity = await _context.Enrollments
                .Include(e => e.Course)
                .Where(e => e.Course != null)
                .GroupBy(e => e.Course.Title)
                .Select(g => new { Title = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(10)
                .ToListAsync();

            vm.PopularityLabels = popularity.Select(p => p.Title).ToList();
            vm.PopularityData = popularity.Select(p => p.Count).ToList();

            // Rating data
            var ratings = courses
                .Select(c => new { 
                    c.Title, 
                    Avg = c.Feedbacks.Any() ? c.Feedbacks.Average(f => f.Rating) : 0 
                })
                .OrderByDescending(x => x.Avg)
                .Take(10)
                .ToList();

            vm.RatingLabels = ratings.Select(r => r.Title).ToList();
            vm.RatingData = ratings.Select(r => r.Avg).ToList();

            return View(vm);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();

            // Admin can delete without checking InstructorId, but cannot edit
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đã xóa khóa học thành công.";
            return RedirectToAction(nameof(Index));
        }
    }
}

using System.Diagnostics;
using FirstAidPlus.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FirstAidPlus.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Data.AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, Data.AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var featuredCourses = _context.Courses
                .Include(c => c.Syllabus)
                    .ThenInclude(s => s.Lessons)
                .Where(c => c.IsPopular)
                .OrderBy(c => c.Id)
                .Take(3)
                .ToList();

            if (!featuredCourses.Any())
            {
                featuredCourses = _context.Courses
                    .Include(c => c.Syllabus)
                        .ThenInclude(s => s.Lessons)
                    .OrderBy(c => c.Id)
                    .Take(3)
                    .ToList();
            }

            var featuredInstructors = _context.Courses
                .Include(c => c.Instructor)
                .Where(c => c.InstructorId != null && c.Instructor != null)
                .Select(c => c.Instructor!)
                .Distinct()
                .Take(3)
                .ToList();

            var testimonials = _context.Testimonials
                .OrderByDescending(t => t.Id)
                .Take(3)
                .ToList();

            var viewModel = new FirstAidPlus.ViewModels.LandingPageViewModel
            {
                FeaturedCourses = featuredCourses,
                FeaturedInstructors = featuredInstructors,
                Testimonials = testimonials
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Contact(string name, string email, string phone, string subject, string message)
        {
            // Here you would typically send an email or save to DB
            TempData["Message"] = "Cảm ơn bạn đã liên hệ. Chúng tôi sẽ phản hồi sớm nhất!";
            return RedirectToAction("Contact");
        }

        public IActionResult Business()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Health()
        {
            return Ok("OK");
        }
    }
}

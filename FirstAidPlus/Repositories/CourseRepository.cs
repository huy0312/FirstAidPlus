using FirstAidPlus.Data;
using FirstAidPlus.Models;
using Microsoft.EntityFrameworkCore;

namespace FirstAidPlus.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly AppDbContext _context;

        public CourseRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Course>> GetAllCoursesAsync()
        {
            var courses = await _context.Courses
                .Include(c => c.Syllabus)
                    .ThenInclude(s => s.Lessons)
                .Include(c => c.Feedbacks)
                .ToListAsync();

            // Auto-categorization for existing data
            bool changed = false;
            foreach (var course in courses.Where(c => string.IsNullOrEmpty(c.Category)))
            {
                if (course.Title.Contains("Cấp cứu")) course.Category = "Cấp cứu";
                else if (course.Title.Contains("Huấn luyện")) course.Category = "Huấn luyện";
                else if (course.Title.Contains("Xử trí")) course.Category = "Xử trí";
                else if (course.Title.Contains("Kỹ năng")) course.Category = "Kỹ năng";
                else if (course.Title.Contains("Phòng ngừa") || course.Title.Contains("Đột quỵ")) course.Category = "Y tế & Sức khỏe";
                else course.Category = "Chung";
                changed = true;
            }

            if (changed)
            {
                await _context.SaveChangesAsync();
            }

            return courses;
        }

        public async Task<(IEnumerable<Course> Courses, int TotalCount, int TotalPages)> GetCoursesAsync(string searchString, string category, string level, int pageNumber = 1, int pageSize = 9)
        {
            var courses = _context.Courses
                .Include(c => c.Syllabus)
                    .ThenInclude(s => s.Lessons)
                .Where(c => c.IsActive)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                courses = courses.Where(s => s.Title.Contains(searchString) || s.Description.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(category) && category != "Tất cả danh mục")
            {
                courses = courses.Where(c => c.Category == category);
            }

            int totalCount = await courses.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            if (pageNumber < 1) pageNumber = 1;

            var pagedCourses = await courses
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (pagedCourses, totalCount, totalPages);
        }

        public async Task<Course?> GetCourseByIdAsync(int id)
        {
            return await _context.Courses
                .Include(c => c.Objectives)
                .Include(c => c.Syllabus)
                    .ThenInclude(s => s.Lessons)
                .Include(c => c.Instructor)
                .Include(c => c.Feedbacks)
                    .ThenInclude(f => f.User)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}

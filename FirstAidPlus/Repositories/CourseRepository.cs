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
            return await _context.Courses
                .Include(c => c.Syllabus)
                    .ThenInclude(s => s.Lessons)
                .Include(c => c.Feedbacks)
                .ToListAsync();
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
                // Simple category filter using CertificateName for demonstration, as Category property is missing
                courses = courses.Where(c => c.CertificateName != null && c.CertificateName.Contains(category));
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

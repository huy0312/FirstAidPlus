using FirstAidPlus.Models;

namespace FirstAidPlus.Repositories
{
    public interface ICourseRepository
    {
        Task<IEnumerable<Course>> GetAllCoursesAsync();
        Task<(IEnumerable<Course> Courses, int TotalCount, int TotalPages)> GetCoursesAsync(string searchString, string category, string level, int pageNumber = 1, int pageSize = 9);
        Task<Course?> GetCourseByIdAsync(int id);
    }
}

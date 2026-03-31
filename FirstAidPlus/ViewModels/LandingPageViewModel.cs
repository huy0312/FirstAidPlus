using FirstAidPlus.Models;

namespace FirstAidPlus.ViewModels
{
    public class LandingPageViewModel
    {
        public List<Course> FeaturedCourses { get; set; } = new List<Course>();
        public List<User> FeaturedInstructors { get; set; } = new List<User>();
        public List<Testimonial> Testimonials { get; set; } = new List<Testimonial>();
    }
}

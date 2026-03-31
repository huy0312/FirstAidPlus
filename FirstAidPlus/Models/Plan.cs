using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstAidPlus.Models
{
    [Table("plans")]
    public class Plan
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public string? Features { get; set; }
        public int DurationValue { get; set; } = 1;
        public string DurationUnit { get; set; } = "Month"; // Week, Month, Year
        
        public ICollection<PlanCourse> PlanCourses { get; set; } = new List<PlanCourse>();
    }
}

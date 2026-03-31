using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstAidPlus.Models
{
    [Table("plan_courses")]
    public class PlanCourse
    {
        [Column("plan_id")]
        public int PlanId { get; set; }
        
        [Column("course_id")]
        public int CourseId { get; set; }

        [ForeignKey("PlanId")]
        public Plan Plan { get; set; }

        [ForeignKey("CourseId")]
        public Course Course { get; set; }
    }
}

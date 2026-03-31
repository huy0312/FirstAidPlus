using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstAidPlus.Models
{
    public class MedicalRecord
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [Required]
        public string ConditionName { get; set; }

        public string Description { get; set; }

        public int YearDiagnosed { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}

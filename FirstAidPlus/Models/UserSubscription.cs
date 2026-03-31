using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstAidPlus.Models
{
    [Table("user_subscriptions")]
    public class UserSubscription
    {
        [Key]
        public int Id { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }
        
        [Column("plan_id")]
        public int PlanId { get; set; }

        [Column("start_date")]
        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        [Column("end_date")]
        public DateTime? EndDate { get; set; }

        public string Status { get; set; } = "Active";

        [ForeignKey("UserId")]
        public User? User { get; set; }

        [ForeignKey("PlanId")]
        public Plan? Plan { get; set; }
    }
}

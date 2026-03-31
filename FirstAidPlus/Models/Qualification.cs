using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstAidPlus.Models
{
    public class Qualification
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        public string? CertificateUrl { get; set; }
        
        public DateTime IssuedAt { get; set; }
        
        public string Status { get; set; } = StatusPending;

        public string? AdminComment { get; set; }

        public const string StatusPending = "Pending";
        public const string StatusApproved = "Approved";
        public const string StatusRejected = "Rejected";

        public int UserId { get; set; }
        
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}

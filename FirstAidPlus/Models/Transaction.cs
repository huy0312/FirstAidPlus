using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstAidPlus.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }

        public int PlanId { get; set; }
        [ForeignKey("PlanId")]
        public Plan? Plan { get; set; }

        public decimal Amount { get; set; }
        public string? OrderDescription { get; set; }

        public string? VnPayTxnRef { get; set; }
        public string? VnPayTransactionNo { get; set; }
        
        public string? MomoOrderId { get; set; }
        public string? MomoTransId { get; set; }

        public string PaymentMethod { get; set; } = "VnPay"; // VnPay, Momo
        
        public string Status { get; set; } = "Pending"; // Pending, Success, Failed
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

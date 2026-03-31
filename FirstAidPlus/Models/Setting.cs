using System.ComponentModel.DataAnnotations;

namespace FirstAidPlus.Models
{
    public class Setting
    {
        [Key]
        public string Key { get; set; } = null!;

        [Required]
        public string Value { get; set; } = null!;

        public string? Description { get; set; }

        public string Group { get; set; } = "General"; // e.g., General, Email, Payment
    }
}

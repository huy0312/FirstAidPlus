using System.ComponentModel.DataAnnotations;

namespace FirstAidPlus.Models
{
    public class Testimonial
    {
        [Key]
        public int Id { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string? StudentRole { get; set; }
        public string? Content { get; set; }
        public int Rating { get; set; }
    }
}

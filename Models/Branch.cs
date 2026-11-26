using System.ComponentModel.DataAnnotations;

namespace Restaurant_Management_System.Models
{
    public class Branch
    {
        public int Id { get; set; }
        [Required] public string Name { get; set; } = "";
        public string? Address { get; set; }
        public string? Phone { get; set; }
    }
}

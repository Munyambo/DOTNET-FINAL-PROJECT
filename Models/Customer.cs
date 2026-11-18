using System.ComponentModel.DataAnnotations;

namespace Restaurant_Management_System.Models
{
    public class Customer
    {
        public int Id { get; set; }
        [Required] public string FullName { get; set; } = "";
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? UserId { get; set; }
    }
}
